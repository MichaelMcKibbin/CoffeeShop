using CoffeeShop.Models;
using CommunityToolkit.Mvvm.Input;

namespace CoffeeShop.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}