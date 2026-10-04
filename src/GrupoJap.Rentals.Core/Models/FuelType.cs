using System.ComponentModel.DataAnnotations;

namespace GrupoJap.Rentals.Models;

public enum FuelType
{
    [Display(Name = "fuel.Unspecified")]
    Unspecified = 0,

    [Display(Name = "fuel.Petrol")]
    Petrol = 1,

    [Display(Name = "fuel.Diesel")]
    Diesel = 2,

    [Display(Name = "fuel.Hybrid")]
    Hybrid = 3,

    [Display(Name = "fuel.Electric")]
    Electric = 4,

    [Display(Name = "fuel.Lpg")]
    Lpg = 5,

    [Display(Name = "fuel.Other")]
    Other = 6
}