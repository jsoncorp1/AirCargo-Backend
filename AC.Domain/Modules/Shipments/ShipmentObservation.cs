namespace AC.Domain.Modules.Shipments;

public enum ShipmentObservation
{
    CustomerRefused,      // No quiere (dispara estado Rejected)
    NoAnswerDay1,         // No contesta día 1
    NoAnswerDay2,         // No contesta día 2
    NoAnswerDay3,         // No contesta día 3
    CustomerTraveling,    // Está de viaje
    WrongPhoneNumber,     // Número incorrecto
    TooFar,               // Muy lejos
    NotDeliveredOnTime,   // No se entregó a tiempo
    InProvince            // En provincia
}
