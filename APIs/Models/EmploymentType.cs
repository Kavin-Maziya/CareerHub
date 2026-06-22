using System.Text.Json.Serialization;

namespace APIs.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EmploymentType
{
    FullTime,
    PartTime,
    Contract,
    Internship
}