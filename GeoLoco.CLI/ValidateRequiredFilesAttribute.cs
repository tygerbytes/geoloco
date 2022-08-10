using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("GeoLoco.Tests")]

namespace GeoLoco.CLI
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ValidateRequiredFilesAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            if (!(value is Program options))
            {
                return ValidationResult.Success;
            }

            // TODO: If these options keep growing, need to break out subcommands
            //  See https://github.com/natemcmaster/CommandLineUtils/tree/main/docs/samples/subcommands

            if (options.CheckVersion)
            {
                return ValidationResult.Success;
            }

            if (options.Check)
            {
                var filesToCheck = new List<bool>
                {
                    options.AddressListCsv != null,
                };

                if (filesToCheck.Count(f => f) > 1)
                {
                    return new ValidationResult("Check one file at a time.");
                }

                if (options.AddressListCsv == null)
                {
                    return new ValidationResult("--check requires a file to check (--address-list-csv <file>)");
                }
            }
            else if (options.AddressListCsv != null)
            {
                if (options.OutputFile == null)
                {
                    return new ValidationResult("Must provide an output file (--output-file <file>)");
                }

                if (
                    !(
                        options.OutputFile.EndsWith(".kml")
                        || options.OutputFile.EndsWith(".csv"))
                    )
                {
                    return new ValidationResult("Output file must be one of (.kml|.csv)");
                }
            }

            if (options.BoundaryKml != null)
            {
                if (!options.BoundaryKml.ToLower().EndsWith(".kml"))
                {
                    return new ValidationResult("--boundary-kml requires <file>.kml");
                }
            }

            return ValidationResult.Success;
        }
    }
}
