namespace MovieQuotes.UI.ViewModels;

using MediatR;
using MovieQuotes.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public abstract class PageViewModelBase : ViewModelBase
{
    [Obsolete("For design-time use only")]
    protected PageViewModelBase()
    {
    }

    protected PageViewModelBase(IMediator mediator, NavigationService navigation) : base(mediator, navigation)
    {
    }
}
