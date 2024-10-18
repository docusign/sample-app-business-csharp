using DocuSign.eSign.Model;
using System.Collections.Generic;
using System;
using DocuSign.MyBusiness.Domain.CustomerProfile.Models;
using DocuSign.MyBusiness.Domain.CustomerProfile.Services.Interfaces;

namespace DocuSign.MyBusiness.Domain.CustomerProfile.Services
{
    public class CustomerProfileEnvelopeBuilder : ICustomerProfileEnvelopeBuilder
    {
        public EnvelopeDefinition BuildUserProfileUpdateEnvelope(
            string eventNotificationUrl,
            string clientUserId,
            string serverTemplateId,
            CustomerProfileSignerInfo signerInfo,
            AccountIdentityVerificationWorkflow identityVerificationWorkflow)
        {
            EnvelopeDefinition envelopeDefinition = new EnvelopeDefinition
            {
                EmailSubject = "REST example - template and recipient authentication",
                EnableWetSign = "false",
                Status = "sent",
                EventNotifications = ConfigureEventNotifications(eventNotificationUrl),

                CompositeTemplates = new List<CompositeTemplate>
                {
                    new CompositeTemplate
                    {
                        ServerTemplates = new List<ServerTemplate>
                        {
                            new ServerTemplate
                            {
                                Sequence = "1",
                                TemplateId = serverTemplateId,
                            }
                        },
                        InlineTemplates = new List<InlineTemplate>
                        {
                            new InlineTemplate
                            {
                                Sequence = "2",
                                Recipients = new Recipients
                                {
                                    Signers = new List<Signer>
                                    {
                                        new Signer
                                        {
                                            Email = signerInfo.SignerEmail,
                                            Name = signerInfo.SignerName,
                                            ClientUserId = clientUserId,
                                            RecipientId = "1",
                                            RoleName = "User",
                                            IdentityVerification = new RecipientIdentityVerification
                                            {
                                                WorkflowId = identityVerificationWorkflow.WorkflowId,
                                                InputOptions = CreateIdentityVerificationInputOptions(identityVerificationWorkflow, signerInfo)
                                            },
                                            Tabs= new Tabs
                                            {
                                                TextTabs= new List<Text>
                                                {
                                                    new Text
                                                    {
                                                        TabLabel = "User.ContactInfo.Primary",
                                                        Value = $"{signerInfo.CountryCode}-{signerInfo.SignerPhone}",
                                                    },
                                                    new Text
                                                    {
                                                        TabLabel = "User.ContactInfo.Secondary",
                                                        Value = signerInfo.SignerEmail,
                                                    },
                                                    new Text
                                                    {
                                                        TabLabel = "User.Addr.Billing.Street",
                                                        Value = "999 3rd Avenue"
                                                    },
                                                    new Text
                                                    {
                                                        TabLabel = "User.Addr.Billing.City",
                                                        Value = "Seattle"
                                                    },
                                                    new Text
                                                    {
                                                        TabLabel = "User.Addr.Billing.State",
                                                        Value = "WA"
                                                    },
                                                    new Text
                                                    {
                                                        TabLabel = "User.Addr.Billing.Postal",
                                                        Value = "98104"
                                                    },
                                                    new Text
                                                    {
                                                        TabLabel = "User.Addr.Shipping.Street",
                                                        Value = "999 3rd Avenue"
                                                    },
                                                    new Text
                                                    {
                                                        TabLabel = "User.Addr.Shipping.City",
                                                        Value = "Seattle"
                                                    },
                                                    new Text
                                                    {
                                                        TabLabel = "User.Addr.Shipping.State",
                                                        Value = "WA"
                                                    },
                                                    new Text
                                                    {
                                                        TabLabel = "User.Addr.Shipping.Postal",
                                                        Value = "98104"
                                                    },
                                                }
                                            }
                                        }

                                    }
                                },
                            },
                        }
                    }
                }
            };

            return envelopeDefinition;
        }

        private static List<EventNotification> ConfigureEventNotifications(string eventNotificationUrl)
        {
            return new List<EventNotification>
                {
                    new EventNotification
                    {
                        Url = eventNotificationUrl,
                        DeliveryMode = "SIM",
                        RequireAcknowledgment = "true",
                        LoggingEnabled = "true",
                        Events = new List<string>
                        {
                            "envelope-sent",
                            "envelope-completed",
                            "envelope-declined",
                            "envelope-voided",
                        },
                        EventData = new ConnectEventData
                        {
                            Version = "restv2.1",
                            Format = "json",
                            IncludeData = new List<string>
                            {
                                "recipients",
                                "tabs"
                            }
                        },
                    }
                };
        }

        private List<RecipientIdentityInputOption> CreateIdentityVerificationInputOptions(AccountIdentityVerificationWorkflow identityVerificationWorkflow, CustomerProfileSignerInfo signerInfo)
        {
            switch (identityVerificationWorkflow.WorkflowLabel)
            {
                case IdentityWorkflowNames.IDVerificationWorkflowLabel:
                    return null;
                case IdentityWorkflowNames.PhoneAuthWorkflowLabel:
                    return
                        new List<RecipientIdentityInputOption>
                        {
                            new RecipientIdentityInputOption
                            {
                                Name = "phone_number_list",
                                ValueType = "PhoneNumberList",
                                PhoneNumberList = new List<RecipientIdentityPhoneNumber>
                                {
                                    new RecipientIdentityPhoneNumber
                                    {
                                        CountryCode = signerInfo.CountryCode,
                                        Number = signerInfo.SignerPhone
                                    }
                                }
                            }
                        };
                default:
                    throw new ApplicationException("The Identity Workflow is not supported");
            }

        }
    }
}
