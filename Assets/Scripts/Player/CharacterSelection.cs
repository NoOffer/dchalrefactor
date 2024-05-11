using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using dchalrefactor.Scripts.Player;

//this class handles the character selection menu
//this class temporarily stores a character selection and when a game is started, it transfers that selection to the PlayerGameDataController
public class CharacterSelection : MonoBehaviour
{
    //VARIABLES------------------------------------------------
    //--------------------------------------------------------UI---------------------------------------------------------------------------------------------
    //Pages and Buttons
    //stores the Character view page - disabled if it is a new user
    public GameObject CharacterViewPage;
    //stores the back button on the Character view page - disabled if continue game, enabled if new game - OLD USER
    public GameObject ChangeCharacterButton;
    //Video Elements
    public VideoClip[] characterClips;
    public RenderTexture[] targetTextures;
    public RawImage showcaseImage;
    public VideoPlayer videoPlayer;
    //----------------------------------------------------CHARACTER------------------------------------------------------------------------------------------
    //stores the selected character
    public string selectedCharacter;
    //stores a reference to the player character data class
    private PlayerCharacterData playerCharacterData;
    //-------------------------------------------------------------------------------------------------------------------------------------------------------
    //METHODS----------------------------------------------------
    void Start ()
    {
        //Initialize a temporary player character data instance to decode the selection
        playerCharacterData = new PlayerCharacterData();
        //initialize selected character to the default character of the user
        selectedCharacter = PlayerGameDataController.Instance.CurrentCharacter;
        //check if new user or old user
        if(PlayerGameDataController.Instance.CheckIfNewUser())
        {
            EnableCharacterPage(false);
        }
        else
        { //since this is an old user, we initialize their character page and assign the correct character to the showcase screen
            InitializeCharacterPageUI();
        }
        UpdateCharacterShowcase();
    }
    //--------------------------------------------------------UI---------------------------------------------------------------------------------------------
    public void InitializeCharacterPageUI() //initializes the character page UI arrangements based on User selection of new game or continue game
    {
        if(!PlayerGameDataController.Instance.CheckIfNewGame())
        {
            //remove the option to change characters
            ChangeCharacterButton.SetActive(false);
        }
    }
    public void EnableCharacterPage(bool decision)
    {
        CharacterViewPage.SetActive(decision);
    }
    //Video
    public void UpdateCharacterShowcase() //from menu selections
    {
        //obtain the index
        int index = playerCharacterData.RetrieveCharacterIndex(selectedCharacter);
        //Retarget the Video entity variables
        videoPlayer.clip = characterClips[index];
        videoPlayer.targetTexture = targetTextures[index];
        //update the target texture of the display holder
        showcaseImage.texture = targetTextures[index];
    }

    //----------------------------------------------------CHARACTER------------------------------------------------------------------------------------------
    //Updated the character selection
    public void SetSelectedCharacter(string character)
    {
        selectedCharacter = character;
    }
    //returns the current selected character
    public string GetCurrentCharacterSelection()
    {
        return selectedCharacter;
    }

    //saves the selection to the PlayerGameDataController
    public void SaveGlobalCharacterSelection()
    {
        PlayerGameDataController.Instance.CurrentCharacter = selectedCharacter;
        PlayerGameDataController.Instance.SaveGameData();
    }

    //---------------------------------------------------------IN-GAME LOGIC------------------------------------------------------------------------
    public void OnGameStart() //called when a gameplay session is started after the Character selection page
    {
        PlayerGameDataController.Instance.IndicateNewUser(false);
    }
}