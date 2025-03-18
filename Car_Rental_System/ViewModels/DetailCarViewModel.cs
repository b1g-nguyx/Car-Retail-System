namespace Car_Rental_System.ViewModels;
public class DetailCarViewModel {
    public CarViewModel CarViewModel { get; set; }
    public IEnumerable<CarViewModel> RelatedCars { get; set; }
}