using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
public class MenuManager : SingletonClass<MenuManager>
{
    [Serializable]
    public enum Menu
    {
        MainMenu = 0,
        Settings = 1,
        ChooseClass = 2,
        ChooseWorld = 3,
        AdvancedChooseWorld = 4,
        Tutorial = 5
    }
    [SerializeField]
    private GameObject mainMenu, settingsMenu, chooseClassMenu,
        chooseWorldMenu, advancedChooseWorldMenu, tutorialMenu;

    private Menu currentMenu = Menu.MainMenu;

    private GameObject GetMenuGO(Menu menu)
    {
        switch (menu)
        {
            case Menu.MainMenu:
                return mainMenu;
            case Menu.Settings:
                return settingsMenu;
            case Menu.ChooseClass:
                return chooseClassMenu;
            case Menu.ChooseWorld:
                return chooseWorldMenu;
            case Menu.AdvancedChooseWorld:
                return advancedChooseWorldMenu;
            case Menu.Tutorial:
                return tutorialMenu;
            default:
                break;
        }
        return null;
    }

    public void OpenMenu(int menuIndex)
    {
        Menu menu = (Menu)menuIndex;
        if (menu == currentMenu)
            return;
        GetMenuGO(currentMenu).SetActive(false);
        GetMenuGO(menu).SetActive(true);
        currentMenu = menu;
    }

    public void Play()
    {
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }
}
