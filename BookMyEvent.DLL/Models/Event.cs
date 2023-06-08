﻿using System;
using System.Collections.Generic;

namespace db.Models;

public partial class Event
{
    public Guid EventId { get; set; }

    public string EventName { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public byte CategoryId { get; set; }

    public DateTime EndDate { get; set; }

    public int Capacity { get; set; }

    public int AvailableSeats { get; set; }

    public string Description { get; set; } = null!;

    public string Location { get; set; } = null!;

    public bool IsPublished { get; set; }

    public bool? IsOffline { get; set; }

    public bool IsCancelled { get; set; }

    public byte MaxNoOfTicketsPerTransaction { get; set; }

    public DateTime CreatedOn { get; set; }

    public bool IsFree { get; set; }

    public bool? IsActive { get; set; }

    public Guid OrganisationId { get; set; }

    public Guid FormId { get; set; }

    public byte RegistrationStatusId { get; set; }

    public Guid CreatedBy { get; set; }

    public Guid AcceptedBy { get; set; }

    public virtual Administration AcceptedByNavigation { get; set; } = null!;

    public virtual EventCategory Category { get; set; } = null!;

    public virtual Administration CreatedByNavigation { get; set; } = null!;

    public virtual Form Form { get; set; } = null!;

    public virtual Organisation Organisation { get; set; } = null!;

    public virtual RegistrationStatus RegistrationStatus { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual ICollection<UserInputForm> UserInputForms { get; set; } = new List<UserInputForm>();
}