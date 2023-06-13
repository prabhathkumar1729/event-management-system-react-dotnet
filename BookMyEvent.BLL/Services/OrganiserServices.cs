﻿using BookMyEvent.BLL.Contracts;
using BookMyEvent.BLL.Models;
using BookMyEvent.DLL.Contracts;
using db.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMyEvent.BLL.Services
{
    public class Organiserservices : IOrganiserServices
    {
        private readonly IAdministrationRepository _administrationRepository;
        private readonly IOrganisationServices _organisationServices;
        private readonly IAccountCredentialsRepository _accountCredentialsRepository;
        public Organiserservices(IAdministrationRepository administrationRepository, IOrganisationServices organisationServices, IAccountCredentialsRepository accountCredentialsRepository)
        {
            _administrationRepository = administrationRepository;
            _organisationServices = organisationServices;
            _accountCredentialsRepository = accountCredentialsRepository;
        }
        public async Task<bool> AcceptOrganiser(Guid administratorId, Guid acceptedBy)
        {
            try
            {
                return await _administrationRepository.UpdateIsAcceptedAndAcceptedBy(acceptedBy, administratorId);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<(bool IsSuccessfull, string Message)> BlockAllOrganisationOrganisers(Guid orgId)
        {
            try
            {
                if (await _administrationRepository.UpdateAllOrganisationOrganisersIsActive(orgId))
                {
                    return (true, "All Organisers Blocked");
                }
                else
                {
                    return (false, "All Organisers Not Blocked");
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool IsSuccessfull, string Message)> BlockOrganiser(Guid administratorId)
        {
            try
            {
                if (await _administrationRepository.ToggleIsActive(administratorId))
                {
                    return (true, "Organiser Blocked");
                }
                else
                {
                    return (false, "Organiser Not Blocked");
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool IsSuccessfull, string Message, BLAdministrator organiser)> CreateSecondaryOwner(BLAdministrator administrator)
        {
            try
            {
                var (IsAvailable, Message) = await IsOrganiserAvailableWithEmail(administrator.Email);
                if (IsAvailable)
                {
                    return (false, Message, null);
                }
                else
                {
                    var mapper = Automapper.InitializeAutomapper();
                    var passModel = await _accountCredentialsRepository.AddCredential(new AccountCredential { Password = administrator.Password, UpdatedOn=DateTime.Now });
                    administrator.AccountCredentialsId = passModel.AccountCredentialsId;
                    Console.WriteLine(administrator.AccountCredentialsId);
                    Console.WriteLine("this is the cred Id ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");
                    if (passModel != null)
                    {
                        var newAdministrator = await _administrationRepository.AddAdministrator(mapper.Map<Administration>(administrator));
                        Console.WriteLine(newAdministrator.AdministratorId);
                        Console.WriteLine("this is the admin Id ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");
                        if (newAdministrator != null)
                        {
                            return (true, "Organiser registration successfull", mapper.Map<BLAdministrator>(newAdministrator));
                        }
                        else
                        {
                            return (false, "Organiser registration unsuccessfull", new BLAdministrator());
                        }
                    }
                    else
                    {
                        return (false, "Organiser password is not createded", new BLAdministrator());
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public async Task<List<BLAdministrator>> GetAllOrganisationOrganisers(Guid orgId)
        {
            try
            {
                var mapper = Automapper.InitializeAutomapper();
                var result = await _administrationRepository.GetAdministrationsByOrgId(orgId);
                return mapper.Map<List<BLAdministrator>>(result);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<BLAdministrator>> GetAllOwners()
        {
            try
            {
                var mapper = Automapper.InitializeAutomapper();
                var result = await _administrationRepository.GetPrimaryAdministrators();
                return mapper.Map<List<BLAdministrator>>(result);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<BLAdministrator>> GetAllRequestedOrganisers(Guid orgId)
        {

            try
            {
                var mapper = Automapper.InitializeAutomapper();
                var result = await _administrationRepository.GetPeerAdministratorRequests(orgId);
                return mapper.Map<List<BLAdministrator>>(result);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<BLAdministrator>> GetAllRequestedOwners()
        {
            try
            {
                var mapper = Automapper.InitializeAutomapper();
                var result = await _administrationRepository.GetPrimaryAdministratorRequests();
                return mapper.Map<List<BLAdministrator>>(result);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<BLAdministrator> GetOrganiserById(Guid administratorId)
        {
            try
            {
                var mapper = Automapper.InitializeAutomapper();
                var result = await _administrationRepository.GetAdministratorById(administratorId);
                return mapper.Map<BLAdministrator>(result);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<(bool IsOrganiserEmailAvailable, string Message)> IsOrganiserAvailableWithEmail(string email)
        {
            try
            {
                if (await _administrationRepository.IsEmailExists(email))
                {
                    return (true, "Email already exists");
                }
                else
                {
                    return (false, "Email doesn't exists");
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<bool> IsOwner(Guid id)
        {
            try
            {
                var result = await _administrationRepository.GetAdministratorById(id);
                if (result.RoleId == 2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<(Guid administratorId, byte roleId, bool IsSuccessfull, string Message)> LoginOrganiser(string email, string password)
        {
            try
            {
                var result = await _administrationRepository.GetAdministratorByEmail(email);
                if (result != null)
                {
                    var res = await _accountCredentialsRepository.CheckPassword(result.AccountCredentialsId, password);
                    if (res)
                    {
                        return (result.AdministratorId, result.RoleId, true, "Login Successfull");
                    }
                    else
                    {
                        return (Guid.Empty, 0, false, "Password is incorrect");
                    }
                }
                else
                {
                    return (Guid.Empty, 0, false, "Email doesn't exists");
                }
            }
            catch (Exception ex)
            {
                return (Guid.Empty, 0, false, ex.Message);
            }
        }

        public async Task<(bool IsSuccessfull, string Message)> RegisterOwner(BLAdministrator owner, BLOrganisation bLOrganisation)
        {
            try
            {
                var mapper = Automapper.InitializeAutomapper();
                var newOrg = await _organisationServices.CreateOrganisation(bLOrganisation);
                if (newOrg.IsSuccessfull)
                {
                    var res = await IsOrganiserAvailableWithEmail(owner.Email);
                    if (res.IsOrganiserEmailAvailable)
                    {
                        return (false, res.Message);
                    }
                    else
                    {
                        var passModel = await _accountCredentialsRepository.AddCredential(new AccountCredential { Password = owner.Password });
                        owner.AccountCredentialsId = passModel.AccountCredentialsId;
                        var newOwner = await _administrationRepository.AddAdministrator(mapper.Map<Administration>(owner));
                        if (newOwner != null)
                        {
                            return (true, "Owner Registered");
                        }
                        else
                        {
                            return (false, "Owner Not Registered");
                        }
                    }
                }
                else
                {
                    return (false, newOrg.Message);
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool IsSuccessfull, string Message)> RegisterPeer(BLAdministrator peer)
        {
            try
            {
                var mapper = Automapper.InitializeAutomapper();
                var res = await IsOrganiserAvailableWithEmail(peer.Email);
                if (res.IsOrganiserEmailAvailable)
                {
                    return (false, res.Message);
                }
                else
                {
                    var newPeer = await _administrationRepository.AddAdministrator(mapper.Map<Administration>(peer));
                    if (newPeer != null)
                    {
                        return (true, "Peer Registered");
                    }
                    else
                    {
                        return (false, "Peer Not Registered");
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<bool> RejectOrganiser(Guid administratorId, Guid rejectedBy, string reason)
        {
            try
            {
                var result = await _administrationRepository.UpdateRejectedByAndIsActive(administratorId, rejectedBy, reason);
                if (result)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<BLAdministrator> UpdateOrganiser(BLAdministrator administrator)
        {
            try
            {
                var mapper = Automapper.InitializeAutomapper();
                var result = await _administrationRepository.UpdateAdministrator(mapper.Map<Administration>(administrator));
                return mapper.Map<BLAdministrator>(result);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
