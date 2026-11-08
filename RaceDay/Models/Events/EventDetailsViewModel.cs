using RaceDay.Models.Categories;

namespace RaceDay.Models.Events;

// PAGE MODEL: the event details page needs an event AND its categories, so this carries both.
public class EventDetailsViewModel
{
    public EventViewModel Event { get; set; } = new();
    public List<CategoryViewModel> Categories { get; set; } = new();
}
