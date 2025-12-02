using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class TourExecution : AggregateRoot  // ✅ NASLEĐUJE AggregateRoot
    {
        // ========== SVOJSTVA SA PRIVATE SETTERIMA - DDD Enkapsulacija ==========

        /// <summary>ID turiste koji izvršava turu</summary>
        public long TouristId { get; private set; }

        /// <summary>ID ture koja se izvršava</summary>
        public long TourId { get; private set; }

        /// <summary>Vreme pokretanja ture</summary>
        public DateTime StartTime { get; private set; }

        /// <summary>Status izvršavanja (Active, Completed, Abandoned)</summary>
        public TourExecutionStatus Status { get; private set; }

        /// <summary>Geografska širina na početku ture (belezi se lokacija turiste)</summary>
        public double StartLatitude { get; private set; }

        /// <summary>Geografska dužina na početku ture (belezi se lokacija turiste)</summary>
        public double StartLongitude { get; private set; }

        /// <summary>Vreme završetka ture (nullable - postavlja se kad se tura završi)</summary>
        public DateTime? CompletionTime { get; private set; }

        /// <summary>Vreme napuštanja ture (nullable - postavlja se kad se tura napusti)</summary>
        public DateTime? AbandonTime { get; private set; }

        // ========== NAVIGATION PROPERTY - Deo Agregata ==========

        /// <summary>
        /// Tura koja se izvršava - deo agregata
        /// Učitava se zajedno sa TourExecution pomoću Include u repozitorijumu
        /// </summary>
        public Tour Tour { get; private set; }

        // ========== PRAZAN KONSTRUKTOR - Za Entity Framework Core ==========

        /// <summary>
        /// Prazan konstruktor za EF Core
        /// Private da spreči direktno instanciranje bez validacije
        /// </summary>
        private TourExecution() { }

        // ========== JAVNI KONSTRUKTOR - Kreiranje Nove Sesije ==========

        /// <summary>
        /// Kreira novu sesiju izvršavanja ture
        /// Validira sve ulazne podatke i postavlja inicijalno stanje
        /// 
        /// Zahtev: "Kada turista pokrene turu, beleži se lokacija turiste"
        /// </summary>
        /// <param name="touristId">ID turiste</param>
        /// <param name="tourId">ID ture</param>
        /// <param name="startLatitude">Geografska širina turiste</param>
        /// <param name="startLongitude">Geografska dužina turiste</param>
        public TourExecution(long touristId, long tourId, double startLatitude, double startLongitude)
        {
            // DOMENSKU VALIDACIJU 1: Koordinate moraju biti validne
            // Zahtev: "front-end prvo dobija lokaciju putem Position simulatora"
            ValidateCoordinates(startLatitude, startLongitude);

            // DOMENSKU VALIDACIJU 2: ID-evi moraju biti pozitivni
            if (touristId <= 0)
                throw new ArgumentException("Tourist ID must be valid.", nameof(touristId));
            if (tourId <= 0)
                throw new ArgumentException("Tour ID must be valid.", nameof(tourId));

            // POSTAVLJANJE INICIJALNOG STANJA AGREGATA
            TouristId = touristId;
            TourId = tourId;
            StartTime = DateTime.UtcNow;
            Status = TourExecutionStatus.Active;  // Nova sesija je uvek Active
            StartLatitude = startLatitude;
            StartLongitude = startLongitude;
            // CompletionTime i AbandonTime ostaju null
        }

        // ========== PRIVATNE METODE - Domensku Pravila ==========

        /// <summary>
        /// Validira da su koordinate u validnom geografskom opsegu
        /// Latitude: -90 do 90
        /// Longitude: -180 do 180
        /// </summary>
        private void ValidateCoordinates(double latitude, double longitude)
        {
            if (latitude < -90 || latitude > 90)
                throw new ArgumentException("Latitude must be between -90 and 90.", nameof(latitude));
            if (longitude < -180 || longitude > 180)
                throw new ArgumentException("Longitude must be between -180 and 180.", nameof(longitude));
        }

        // ========== JAVNE METODE - Domensku Operacije ==========

        /// <summary>
        /// Završava izvršavanje ture
        /// Biznis pravilo: Samo aktivne sesije mogu biti završene
        /// </summary>
        public void Complete()
        {
            // BIZNIS PRAVILO: Samo Active status može preći u Completed
            if (Status != TourExecutionStatus.Active)
                throw new InvalidOperationException("Only active tour executions can be completed.");

            // Promena stanja agregata
            Status = TourExecutionStatus.Completed;
            CompletionTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Napušta izvršavanje ture
        /// Biznis pravilo: Samo aktivne sesije mogu biti napuštene
        /// </summary>
        public void Abandon()
        {
            // BIZNIS PRAVILO: Samo Active status može preći u Abandoned
            if (Status != TourExecutionStatus.Active)
                throw new InvalidOperationException("Only active tour executions can be abandoned.");

            // Promena stanja agregata
            Status = TourExecutionStatus.Abandoned;
            AbandonTime = DateTime.UtcNow;
        }
    }
}
