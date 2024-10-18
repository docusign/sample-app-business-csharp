using DocuSign.eSign.Model;
using DocuSign.MyBusiness.Domain.Common.Models;
using DocuSign.MyBusiness.Domain.CustomQuote.Services.Interfaces;
using System.Collections.Generic;

namespace DocuSign.MyBusiness.Domain.CustomQuote.Services
{
    public class CustomQuoteEnvelopeBuilder : ICustomQuoteEnvelopeBuilder
    {
        public EnvelopeDefinition BuildCustomQuoteEnvelope(string eventNotificationUrl, SignerInfo signerInfo)
        {
            EnvelopeDefinition envelopeDefinition = new EnvelopeDefinition
            {
                EmailSubject = "REST example - direct doc contribution, anchors, SMS delivery, and metadata",
                Status = "sent",
                EventNotifications = ConfigureEventNotifications(eventNotificationUrl),
                CompositeTemplates = new List<CompositeTemplate>
                {
                    new CompositeTemplate
                    {
                        CompositeTemplateId = "1",
                        Document = new Document
                        {
                            DocumentId = "1",
                            Name = "Order Form",
                            DocumentFields = new List<NameValue>
                            {
                                new NameValue
                                {
                                    Name = "source",
                                    Value = "generated"
                                }
                            },
                        },
                        InlineTemplates = new List<InlineTemplate>
                        {
                            new InlineTemplate
                            {
                                Sequence = "1",
                                CustomFields = new CustomFields
                                {
                                    TextCustomFields = new List<TextCustomField>
                                    {
                                        new TextCustomField
                                        {
                                            Name = "Team",
                                            Show = "true",
                                            Value = "EMEA-MM"
                                        }
                                    }
                                },
                                Recipients = new Recipients
                                {
                                    Signers = new List<Signer>
                                    {
                                        new Signer
                                        {
                                            Name = signerInfo.FullName,
                                            RecipientId = "1",
                                            RoutingOrder= "1",
                                            DeliveryMethod = "SMS",
                                            PhoneNumber = new RecipientPhoneNumber
                                            {
                                                CountryCode = signerInfo.CountryCode.Replace("+", string.Empty),
                                                Number = signerInfo.PhoneNumber
                                            },
                                            Tabs = new Tabs
                                            {
                                                SignHereTabs = new List<SignHere>
                                                {
                                                    new SignHere
                                                    {
                                                        DocumentId= "1",
                                                        TabLabel = "Purchaser.Signature",
                                                        AnchorIgnoreIfNotPresent = "true",
                                                        AnchorString = "$P_SH"
                                                    }
                                                },
                                                FullNameTabs = new List<FullName>
                                                {
                                                    new FullName
                                                    {
                                                        DocumentId= "1",
                                                        TabLabel = "Purchaser.Name",
                                                        AnchorIgnoreIfNotPresent = "true",
                                                        AnchorString = "$P_FN"
                                                    }
                                                },
                                                DateSignedTabs = new List<DateSigned>
                                                {
                                                    new DateSigned
                                                    {
                                                        DocumentId= "1",
                                                        TabLabel = "Purchaser.Signature.Date",
                                                        AnchorIgnoreIfNotPresent = "true",
                                                        AnchorString = "$P_DS"
                                                    }
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
    }
}
