using Microsoft.Office.Server.Search.ContentProcessingEnrichment;
using Microsoft.Office.Server.Search.ContentProcessingEnrichment.PropertyTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ContentEnrichmentService.Service
{
#if DEBUG
    // Reduces the maximum amount of parallel threads to 1, makes it a lot easier to debug
    // Exception callstacks are transmitted in DEBUG builds
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single, IncludeExceptionDetailInFaults = true)]
#endif
    public class ContentProcessor : IContentProcessingEnrichmentService
    {

        private static string[] Categories = { "Cat 1", "Cat 2", "Cat 3", "Cat 4", "Cat 5" };
        private static int Counter = 0;
        public ProcessedItem ProcessItem(Item item)
        {
            var originalPath = GetItemProperty(item, "OriginalPath");

            Trace.WriteLine($"Enriching '{originalPath}'...");

            var fileExtension = GetItemProperty(item, "FileExtension");

            var props = new List<AbstractProperty>();
            props.Add(new Property<string>
            {
                Name = "CustomCategory",
                Value = Categories[Counter++ % 5]
            });

            if (fileExtension != null)
            {
                Trace.WriteLine($"Extension '{fileExtension}'...");
                props.Add(new Property<string>
                {
                    Name = "CustomExt",
                    Value = fileExtension
                });
            }
            return new ProcessedItem
            {
                ErrorCode = 0,
                ItemProperties = props
            };
        }

        private static string GetItemProperty(Item item, string propertyName)
        {
            var originalPathProperty = item.ItemProperties.FirstOrDefault(p => p.Name == propertyName);

            if (originalPathProperty == null)
            {
                string msg = $"{propertyName} property is missing in the item's properties";
                Trace.WriteLine(msg);
                throw new NullReferenceException(msg);
            }

            var value = originalPathProperty.ObjectValue as string;

            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }

            {
                string msg = $"{propertyName} property has no value";
                Trace.WriteLine(msg);
                return null;
            }
        }
    }
}
