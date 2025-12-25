using BusinessLogic;
using Repository;
using Models;

namespace BusinessFlow;

public class EvacuationPlanningBusinessFlow
{
    private readonly EvacuationPlanningRepository _evacuationPlanningRepository;
    public EvacuationPlanningBusinessFlow(EvacuationPlanningRepository evacuationPlanningRepository)
    {
        _evacuationPlanningRepository = evacuationPlanningRepository;
    }

    public EvacuationZoneResponse ProcessEvacuationZone(EvacuationZoneRequest request)
    {
        Console.WriteLine("BusinessFlow");
        EvacuationZoneBusinessLogic.ValidateEvacuationZoneRequest(request);

        LocationCoordinate locationCoordinate = new LocationCoordinate()
        {
            Latitude = request.Latitude,
            Longitude = request.Longitude
        };

        _evacuationPlanningRepository.AddLocationCoordinate(locationCoordinate);

        EvacuationZone evacuationZone = new EvacuationZone()
        {
            ZoneID = request.ZoneID,
            LocationCoordinateID = locationCoordinate.Id,
            NumberOfPeople = request.NumberOfPeople,
            UrgencyLevel = request.UrgencyLevel
        };

        _evacuationPlanningRepository.AddEvacautionZone(evacuationZone);

        EvacuationZoneResponse response = new EvacuationZoneResponse()
        {
            ZoneID = request.ZoneID,
            LocationCoordinate = new LocationCoordinate()
            {
                Latitude = request.Latitude,
                Longitude = request.Longitude
            },
            NumberOfPeople = request.NumberOfPeople,
            UrgencyLevel = request.UrgencyLevel
        };

       return response;
    }


    // call repository methods

    // handle responses

}