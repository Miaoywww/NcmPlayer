﻿using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using NonsPlayer.Contracts.Services;
using NonsPlayer.Core;
using NonsPlayer.Core.Services;

namespace NonsPlayer.ViewModels;

public partial class PersonalCenterViewModel : ObservableRecipient
{
    public AccountService Account => AccountService.Instance;
    public INavigationService NavigationService
    {
        get;
    }

    public PersonalCenterViewModel(INavigationService navigationService)
    {
        NavigationService = navigationService;
    }

    public void PersonalCenterPage_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (!Nons.Instance.isLoggedin)
        {
            NavigationService.NavigateTo(typeof(LoginViewModel).FullName!);
        }
    }
}