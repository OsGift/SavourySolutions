﻿namespace SavourySolutions.Services.Data
{
    using System.Threading.Tasks;

    using SavourySolutions.Common;
    using SavourySolutions.Data.Common.Repositories;
    using SavourySolutions.Data.Models;
    using SavourySolutions.Models.ViewModels.Contacts;
    using SavourySolutions.Services.Data.Contracts;
    using SavourySolutions.Services.Messaging;

    public class ContactsService : IContactsService
    {
        private readonly IRepository<ContactFormEntry> userContactsRepository;
        private readonly IEmailSender emailSender;

        public ContactsService(
            IRepository<ContactFormEntry> userContactsRepository,
            IEmailSender emailSender)
        {
            this.userContactsRepository = userContactsRepository;
            this.emailSender = emailSender;
        }

        public async Task SendContactToAdminAsync(ContactFormEntryViewModel contactFormEntryViewModel)
        {
            var contactFormEntry = new ContactFormEntry
            {
                FirstName = contactFormEntryViewModel.FirstName,
                LastName = contactFormEntryViewModel.LastName,
                Email = contactFormEntryViewModel.Email,
                Subject = contactFormEntryViewModel.Subject,
                Content = contactFormEntryViewModel.Content,
            };

            await this.userContactsRepository.AddAsync(contactFormEntry);
            await this.userContactsRepository.SaveChangesAsync();

            await this.emailSender.SendEmailAsync(
                contactFormEntryViewModel.Email,
                string.Concat(contactFormEntryViewModel.FirstName, " ", contactFormEntryViewModel.LastName),
                GlobalConstants.SystemEmail,
                contactFormEntryViewModel.Subject,
                contactFormEntryViewModel.Content);
        }
    }
}
