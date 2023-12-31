﻿using BookMyEvent.BLL.Contracts;
using BookMyEvent.BLL.Models;
using BookMyEvent.BLL.RequestModels;
using db.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BookMyEvent.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventServices _eventServices;
        public EventController(IEventServices eventServices)
        {
            _eventServices = eventServices;
        }
        /// <summary>
        /// Service to Add an Event
        /// </summary>
        /// <returns>Retuns Added event Details</returns>
        [Authorize(Roles = "Owner,Peer,Secondary_Owner")]
        [HttpPost("addevent")]
        public async Task<IActionResult> AddNewEvent()
        {
            try
            {

                List<BLEventImages> bLEventImages = new List<BLEventImages>();
                var images = Request.Form.Files;
                if (images != null && images.Count > 0)
                {
                    foreach (var image in images)
                    {
                        var memoryStream = new MemoryStream();
                        await image.CopyToAsync(memoryStream);
                        var imageBody = memoryStream.ToArray();
                        bLEventImages.Add(new BLEventImages()
                        {
                            ImgBody = imageBody,
                            ImgName = "CoverImage",
                            ImgType = "CoverImage"
                        });
                    }
                    bLEventImages[0].ImgType = "profile";
                    bLEventImages[0].ImgName = "profile";

                }
                BLEvent bLEvent = new();
                bLEvent.EventName = Request.Form["EventName"];
                bLEvent.StartDate = DateTime.Parse(Request.Form.First(e => e.Key == "StartDate").Value);
                bLEvent.EndDate = DateTime.Parse(Request.Form.First(e => e.Key == "EndDate").Value);
                bLEvent.CategoryId = byte.Parse(Request.Form.First(e => e.Key == "CategoryId").Value);
                bLEvent.Capacity = int.Parse(Request.Form.First(e => e.Key == "Capacity").Value);
                bLEvent.AvailableSeats = int.Parse(Request.Form.First(e => e.Key == "AvailableSeats").Value);
                bLEvent.ProfileImgBody = bLEventImages[0].ImgBody;
                bLEvent.Description = Request.Form.First(e => e.Key == "Description").Value;
                bLEvent.Location = Request.Form.First(e => e.Key == "Location").Value;
                bLEvent.Country = Request.Form.First(e => e.Key == "Country").Value;
                bLEvent.State = Request.Form.First(e => e.Key == "State").Value;
                bLEvent.City = Request.Form.First(e => e.Key == "City").Value;
                bLEvent.IsPublished = bool.Parse(Request.Form.First(e => e.Key == "IsPublished").Value);
                bLEvent.IsCancelled = false;
                bLEvent.MaxNoOfTicketsPerTransaction = byte.Parse(Request.Form.First(e => e.Key == "MaxNoOfTicketsPerTransaction").Value);
                bLEvent.CreatedOn = DateTime.Parse(Request.Form.First(e => e.Key == "CreatedOn").Value);
                bLEvent.UpdatedOn = DateTime.Parse(Request.Form.First(e => e.Key == "UpdatedOn").Value);
                bLEvent.IsFree = bool.Parse(Request.Form.First(e => e.Key == "IsFree").Value);
                bLEvent.EventStartingPrice = int.Parse(Request.Form.First(e => e.Key == "EventStartingPrice").Value);
                bLEvent.EventEndingPrice = int.Parse(Request.Form.First(e => e.Key == "EventEndingPrice").Value);
                bLEvent.IsActive = true;
                bLEvent.OrganisationId = Guid.Parse(Request.Form.First(e => e.Key == "OrganisationId").Value);
                bLEvent.FormId = Guid.Parse(Request.Form.First(e => e.Key == "FormId").Value);
                bLEvent.RegistrationStatusId = byte.Parse(Request.Form.First(e => e.Key == "RegistrationStatusId").Value);
                bLEvent.CreatedBy = Guid.Parse(Request.Form.First(e => e.Key == "CreatedBy").Value);
                if (Request.Form.Where(e => e.Key == "AcceptedBy").ToList().Count() > 0)
                {
                    bLEvent.AcceptedBy = Guid.Parse(Request.Form.First(e => e.Key == "AcceptedBy").Value);
                }
                bLEventImages.RemoveAt(0);
                (BLEvent Event, string message) AddNewEvent = await _eventServices.Add(bLEvent, bLEventImages);
                if (AddNewEvent.Event != null) { return Ok(AddNewEvent.Event); }
                return BadRequest(AddNewEvent.message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Service to Get Event By Id
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns> event Details</returns>
        [AllowAnonymous]
        [HttpGet("geteventbyid")]
        public async Task<IActionResult> GetEventById(Guid eventId)
        {
            try
            {
                BLEvent _event = await _eventServices.GetEventById(eventId);
                if (_event == null) { return BadRequest("error in BL"); }
                return Ok(_event);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Service to Update Event
        /// </summary>
        /// <param name="newEvent"></param>
        /// <returns>Updated Event Details</returns>
        [Authorize(Roles = "Owner,Peer,Secondary_Owner")]
        [HttpPut("updateevent")]
        public async Task<IActionResult> UpdateEvent([FromBody] BLEvent newEvent)
        {
            try
            {
                (BLEvent _event, string message) updatedEvent = await _eventServices.UpdateEvent(newEvent);
                if (updatedEvent._event == null) { return BadRequest(updatedEvent.message); }
                return Ok(updatedEvent._event);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Service to Delete an event
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns>true if deleted else false along with confirmation message</returns>
        [Authorize(Roles = "Owner,Peer,Secondary_Owner")]
        [HttpDelete("deleteeventpermanently")]
        public async Task<IActionResult> DeleteEventPermanently(Guid eventId)
        {
            try
            {
                (bool isDeleted, string message) deleteEvent = await _eventServices.DeleteEvent(eventId);
                if (!deleteEvent.isDeleted) { return BadRequest(deleteEvent.message); }
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Service to Get all active published events
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns>list of published events</returns>
        [AllowAnonymous]
        [HttpGet("GetAllActivePublishedEvents")]
        public async Task<IActionResult> GetAllActivePublishedEvents(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                int skipCount = (pageNumber - 1) * pageSize;
                List<BLEvent> paginatedEvents = await _eventServices.GetAllActivePublishedEvents(pageNumber, pageSize);
                if (paginatedEvents == null)
                {
                    return BadRequest("Error in BLL");
                }
                return Ok(paginatedEvents);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Service to Get all active published events by orgId
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns>
        /// Returns list of published events by orgId
        /// </returns>
        [AllowAnonymous]
        [HttpGet("GetAllActivePublishedEventsByOrgId")]
        public async Task<IActionResult> GetAllActivePublishedEventsByOrgId(Guid orgId)
        {
            try
            {
                List<BLEvent> AllEvents = await _eventServices.GetAllActivePublishedEventsByOrgId(orgId);
                if (AllEvents == null) { return BadRequest("error in BL"); }
                return Ok(AllEvents);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Service to Get all active published events by location
        /// </summary>
        /// <param name="location"></param>
        /// <returns>
        /// Returns list of published events by location
        /// </returns>
        [AllowAnonymous]
        [HttpGet("GetAllActivePublishedEventsByLocation")]
        public async Task<IActionResult> GetAllActivePublishedEventsByLocation(string location)
        {
            try
            {
                List<BLEvent> AllEvents = await _eventServices.GetAllActivePublishedEventsByLocation(location);
                if (AllEvents == null) { return BadRequest("error in BL"); }
                return Ok(AllEvents);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Service to update event registration status
        /// </summary>
        /// <param name="_event"></param>
        /// <returns>
        /// Returns updated event details
        /// </returns>
        [Authorize(Roles = "Owner,Peer,Secondary_Owner")]
        [HttpPut("{eventId}/RegistrationStatus")]
        public async Task<IActionResult> UpdateEventRegistrationStatus(BLEvent _event)
        {
            try
            {
                BLEvent Events = await _eventServices.UpdateEventRegistrationStatus(_event.EventId, _event.RegistrationStatusId, _event.UpdatedBy.Value, DateTime.Now);
                if (Events == null) { return BadRequest("error in BL"); }
                return Ok(Events);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Service to update event cancellation status
        /// </summary>
        /// <param name="_event"></param>
        /// <returns>
        /// Returns updated event details
        /// </returns>
        [Authorize(Roles = "Owner,Peer,Secondary_Owner")]
        [HttpPut("{eventId}/CancelEvent")]
        public async Task<IActionResult> UpdateIsCancelledEvent(BLEvent _event)
        {
            try
            {
                BLEvent Events = await _eventServices.UpdateIsCancelledEvent(_event.EventId, _event.UpdatedBy.Value, DateTime.Now);
                if (Events == null) { return BadRequest("error in BL"); }
                return Ok(Events);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Service to update event publish status
        /// </summary>
        /// <param name="_event"></param>
        /// <returns>
        /// Returns updated event details
        /// </returns>
        [Authorize(Roles = "Owner,Peer,Secondary_Owner")]
        [HttpPut("{eventId}/Publish")]
        public async Task<IActionResult> UpdateIsPublishedEvent(BLEvent _event)
        {
            try
            {
                BLEvent Events = await _eventServices.UpdateIsPublishedEvent(_event.EventId, _event.UpdatedBy.Value, DateTime.Now);
                if (Events == null) { return BadRequest("error in BL"); }
                return Ok(Events);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Service to update event accept status
        /// </summary>
        /// <param name="_event"></param>
        /// <returns>
        /// Return updated event details
        /// </returns>
        [Authorize(Roles = "Owner,Peer,Secondary_Owner")]
        [HttpPut("{eventId}/Accept")]
        public async Task<IActionResult> UpdateAcceptedBy(BLEvent _event)
        {
            try
            {
                BLEvent Events = await _eventServices.UpdateAcceptedBy(_event.EventId, _event.AcceptedBy.Value, _event.UpdatedBy.Value, DateTime.Now);
                if (Events == null) { return BadRequest("error in BL"); }
                return Ok(Events);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Service to update event reject status
        /// </summary>
        /// <param name="_event"></param>
        /// <returns>
        /// Returns updated event details
        /// </returns>
        [Authorize(Roles = "Owner,Peer,Secondary_Owner")]
        [HttpPut("{eventId}/Reject")]
        public async Task<IActionResult> UpdateRejectedBy(BLEvent _event)
        {
            try
            {
                BLEvent Events = await _eventServices.UpdateRejectedBy(_event.EventId, _event.RejectedBy.Value, _event.UpdatedBy.Value, DateTime.Now, _event.RejectedReason);
                if (Events == null) { return BadRequest("error in BL"); }
                return Ok(Events);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Service to get all active events by organisation
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns>
        /// Returns list of events by organisation
        /// </returns>
        [AllowAnonymous]
        [HttpGet("GetAllCreatedEventsByOrganisation")]
        public async Task<IActionResult> GetAllCreatedEventsByOrganisation(Guid orgId)
        {
            try
            {
                List<BLEvent> AllEvents = await _eventServices.GetAllCreatedEventsByOrganisation(orgId);
                if (AllEvents == null) { return BadRequest("error in BL"); }
                return Ok(AllEvents);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Service to get all active events by organiser
        /// </summary>
        /// <param name="organiserId"></param>
        /// <returns>
        /// Returns list of events by organiser
        /// </returns>
        [Authorize(Roles = "Owner,Peer,Secondary_Owner")]
        [HttpGet("GetAllCreatedEventsByOrganiser")]
        public async Task<IActionResult> GetAllCreatedEventsByOrganiser(Guid organiserId)
        {
            try
            {
                List<BLEvent> AllEvents = await _eventServices.GetAllCreatedEventsByOrganiser(organiserId);
                if (AllEvents == null) { return BadRequest("error in BL"); }
                return Ok(AllEvents);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Service to get all active events by filter
        /// </summary>
        /// <param name="filterEvent"></param>
        /// <returns>
        /// Returns list of events by filter
        /// </returns>
        [AllowAnonymous]
        [HttpPost("GetFilteredEvents")]
        public async Task<IActionResult> GetFilteredEvents(FilterEvent filterEvent)
        {
            try
            {
                // Set default values if parameters are not provided
                Console.WriteLine(filterEvent.startPrice + " " + filterEvent.pageSize + filterEvent.endDate + filterEvent.startPrice + "---");

                filterEvent.startDate = filterEvent.startDate == default ? DateTime.Now.AddYears(-20) : filterEvent.startDate;
                filterEvent.endDate = filterEvent.endDate == default ? DateTime.Now.AddYears(20) : filterEvent.endDate;
                filterEvent.startPrice = filterEvent.startPrice == default ? 0 : filterEvent.startPrice;
                filterEvent.endPrice = filterEvent.endPrice == default ? 214748367 : filterEvent.endPrice;
                filterEvent.location = filterEvent.location ?? string.Empty;
                filterEvent.name = filterEvent.name ?? string.Empty;
                filterEvent.categoryIds = filterEvent.categoryIds ?? new List<int>();
                filterEvent.pageNumber = filterEvent.pageNumber == default ? 0 : filterEvent.pageNumber;
                filterEvent.pageSize = filterEvent.pageSize == default ? 10 : filterEvent.pageSize;
                var result = await _eventServices.GetFilteredEvents(filterEvent.startDate, filterEvent.endDate, filterEvent.startPrice, filterEvent.endPrice, filterEvent.location, filterEvent.name, filterEvent.isFree, filterEvent.categoryIds, filterEvent.pageNumber, filterEvent.pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Service to delete event
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="orgnaniserId"></param>
        /// <returns>
        /// Returns message of event deleted
        /// </returns>
        [Authorize(Roles = "Owner,Peer,Secondary_Owner")]
        [HttpDelete("{eventId}/deletedBy/{orgnaniserId}")]
        public async Task<IActionResult> DeleteEvent(Guid eventId, Guid orgnaniserId)
        {
            try
            {
                var result = await _eventServices.SoftDelete(eventId, orgnaniserId, DateTime.Now);
                if (result.isActiveUpdated)
                {
                    return Ok(result.message);
                }
                else
                {
                    return BadRequest(result.message);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Service to get past events by organisation
        /// </summary>
        /// <param name="organisationId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns>
        /// Returns list of past events by organisation
        /// </returns>
        [Authorize(Roles = "Owner,Peer,Secondary_Owner")]
        [HttpGet("OrganisationPastEvents/{organisationId}")]
        public async Task<IActionResult> GetOrganisationPastEvents(Guid organisationId, int pageNumber, int pageSize)
        {
            try
            {
                var result = await _eventServices.GetAllPastEventsByOrganisationId(organisationId, pageNumber, pageSize);
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest("No past events found");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Service to get past events by organiser
        /// </summary>
        /// <param name="organiserId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns>
        /// Retudns list of past events by organiser
        /// </returns>
        [Authorize(Roles = "Owner,Peer,Secondary_Owner")]
        [HttpGet("OrganiserPastEvents/{organiserId}")]
        public async Task<IActionResult> GetOrganiserPastEvents(Guid organiserId, int pageNumber, int pageSize)
        {
            try
            {
                var result = await _eventServices.GetAllPastEventsByOrganiserId(organiserId, pageNumber, pageSize);
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest("No past events found");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Setvice to get number of past events by organiser and organisation
        /// </summary>
        /// <param name="organiserId"></param>
        /// <param name="organisationId"></param>
        /// <returns></returns>
        [HttpGet("GetNoOfPastEvents/{organiserId}/{organisationId}")]
        public async Task<IActionResult> GetNoOfPastEvents(Guid organiserId, Guid organisationId)
        {
            try
            {
                (int organiserPastEvents, int organisationPastEvents) NoOfPastEvents = await _eventServices.GetNoOfPastEvents(organiserId, organisationId);
                return Ok(new
                {
                    organiserEvents = NoOfPastEvents.organiserPastEvents,
                    organisationEvents = NoOfPastEvents.organisationPastEvents
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Service to get event requests in organisation
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Owner,Peer,Secondary_Owner,Admin")]
        [HttpGet("OrganisationRequests/{orgId}")]
        public async Task<IActionResult> GetOrganisationRequests(Guid orgId)
        {
            try
            {
                var result = await _eventServices.GetOrganisationRequestedEvents(orgId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Service to get number of event requests in organisation
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Owner,Peer,Secondary_Owner,Admin")]
        [HttpGet("NoOfOrganisationRequests/{orgId}")]
        public async Task<IActionResult> GetNoOfOrganisationRequests(Guid orgId)
        {
            try
            {
                var result = await _eventServices.GetNoOfOrganisationRequestedEvents(orgId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Service to get event requests in organiser
        /// </summary>
        /// <param name="organiserId"></param>
        /// <returns>
        /// Returns list of event requests in organiser
        /// </returns>
        [Authorize(Roles = "Owner,Peer,Secondary_Owner,Admin")]
        [HttpGet("OrganiserRequests/{organiserId}")]
        public async Task<IActionResult> GetOrganiserRequests(Guid organiserId)
        {
            try
            {
                var result = await _eventServices.GetOrganiserRequestedEvents(organiserId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Service to get event images
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns>
        /// Returns list of event images
        /// </returns>
        [AllowAnonymous]
        [HttpGet("geteventimages/{eventId}")]
        public async Task<IActionResult> GetEventImages(Guid eventId)
        {
            try
            {
                var images = await _eventServices.GetEventImages(eventId);
                return Ok(images);
            }
            catch
            {
                return BadRequest("error");
            }
        }
    }
}
