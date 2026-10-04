using System.ComponentModel.DataAnnotations;

namespace GrupoJap.Rentals.Models;

public enum FuelType
{
    [Display(Name = "Por selecionar")]
    Unspecified = 0,

    [Display(Name = "Gasolina")]
    Petrol = 1,

    [Display(Name = "Gasóleo")]
    Diesel = 2,

    [Display(Name = "Híbrido")]
    Hybrid = 3,

    [Display(Name = "Elétrico")]
    Electric = 4,

    [Display(Name = "GPL")]
    Lpg = 5,

    [Display(Name = "Outro")]
    Other = 6
}