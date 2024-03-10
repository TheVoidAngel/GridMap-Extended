using UnityEngine;
using UnityEngine.Windows;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Windows.Speech;
using System.Linq;
using System;
public class VRCharacterController : MonoBehaviour
{
    public VRCSVFileReader FileReader; 

    private KeywordRecognizer KeywordRecognizer;
    private Dictionary<string, Action> Actions = new Dictionary<string, Action>();
    private Controls Inputs;
    private Vector2 move = Vector2.zero;
    public float InputRate = 0.25f;
    private bool jumping = false; 
    // Start is called before the first frame update
    void Awake ()
    {

        //physical interface control setup (keyboard, controller)
        Inputs = new Controls();
        Inputs.PlayerInputs.Move.performed += cntxt => move = cntxt.ReadValue<Vector2>();
        Inputs.PlayerInputs.Move.canceled += cntxt => move = Vector2.zero;
        Inputs.PlayerInputs.SpeakAroundMe.performed += cntxt => SpeakAroundMe();
        Inputs.PlayerInputs.SpeakCoordinates.performed += cntxt => SpeakCoordinates();
        Inputs.PlayerInputs.SpeakLandmarks.performed += cntxt => SpeakLandmarks();
        Inputs.PlayerInputs.SpeakAmbiant.performed += cntxt => SpeakAmbientSounds();
        Inputs.PlayerInputs.ToggleAmbiant.performed += cntxt => ToggleAmbientSounds();
        Inputs.PlayerInputs.Enable();
        StartCoroutine(MoveInput());

        //voice command control setup 
        Actions.Add("up", MoveForward);
        Actions.Add("down", MoveBackward);
        Actions.Add("right", MoveRight);
        Actions.Add("left", MoveLeft);
        Actions.Add("jump", Jump);
        Actions.Add("speak coordinates", SpeakCoordinates);
        Actions.Add("speak landmarks", SpeakLandmarks);
        Actions.Add("speak ambiant sounds", SpeakAmbientSounds);
        Actions.Add("toggle ambiant sounds", ToggleAmbientSounds);
        KeywordRecognizer = new KeywordRecognizer(Actions.Keys.ToArray());
        KeywordRecognizer.OnPhraseRecognized += Recognized;
        KeywordRecognizer.Start();

        //lock character to grid
        transform.position = new Vector3(transform.position.x - (transform.position.x % FileReader.GridSpacing), transform.position.y,
            transform.position.z - (transform.position.z % FileReader.GridSpacing));
            
    }
    private void Recognized(PhraseRecognizedEventArgs Word)
    {

        Debug.Log(Word.text);
        Actions[Word.text].Invoke();
    }
    // Update is called once per frame
    void Update()
    {

    }
    private void OnApplicationQuit()
    {
        KeywordRecognizer.Stop();
        Inputs.PlayerInputs.Disable();
    }
    private IEnumerator MoveInput()
    {
        while (true)
        {
           // Debug.Log(move);
            if (move.x < 0)
                MoveLeft();
            if (move.x > 0)
                MoveRight();
            if (move.y < 0)
                MoveBackward();
            if (move.y > 0)
                MoveForward();
            yield return new WaitForSeconds(InputRate);
        }
    }
    private bool CheckMove(Vector3 vec)
    {
        return Physics.Raycast(transform.position, vec, FileReader.GridSpacing);
    }
    private void Jump()
    {
        jumping = true; 
    }
    private void JumpMovement(Vector3 vector)
    {
        while (true)
        {
            if (CheckMove(vector))
                break;
            if (vector.x < 0)
                MoveLeft();
            if (vector.x > 0)
                MoveRight();
            if (vector.z < 0)
                MoveBackward();
            if (vector.z > 0)
                MoveForward();
        }
        jumping = false;
    }
    private void MoveForward()
    {
       if (!CheckMove(Vector3.forward))
          transform.Translate(Vector3.forward * FileReader.GridSpacing);
       if (jumping)
            JumpMovement(Vector3.forward);
    }
    private void MoveBackward()
    {
        if (!CheckMove(Vector3.back))
            transform.Translate(Vector3.back * FileReader.GridSpacing);
        if (jumping)
            JumpMovement(Vector3.back);
    }
    private void MoveLeft()
    {
        if (!CheckMove(Vector3.left))
            transform.Translate(Vector3.left * FileReader.GridSpacing);
        if (jumping)
            JumpMovement(Vector3.left);

    }
    private void MoveRight()
    {
        if (!CheckMove(Vector3.right))
            transform.Translate(Vector3.right * FileReader.GridSpacing);
        if (jumping)
            JumpMovement(Vector3.right);
    }
    private void SpeakAroundMe()
    {

    }
    private void SpeakCoordinates()
    {

    }
    private void SpeakLandmarks()
    {

    }
    private void SpeakAmbientSounds()
    {

    }
    private void ToggleAmbientSounds()
    {

    }

}
