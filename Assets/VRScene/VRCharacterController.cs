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
    public RaycastHit hit;
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
        
        Physics.Raycast(transform.position, vec,out hit, FileReader.GridSpacing);
        if (hit.transform != null)
            hit.transform.GetComponent<AudioSource>().Play();
        //Debug.Log(hit.transform.name);
        //put sound code here

        return Physics.Raycast(transform.position, vec, FileReader.GridSpacing);
    }
    private IEnumerator Move(Vector3 p1, Vector3 p2, float time)
    {
        float timer = 0;
        while (timer <= time)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Slerp(p1, p2, Mathf.SmoothStep(0f, 1f, timer / time));
            yield return new WaitForEndOfFrame();
        }
        transform.position = Vector3.Slerp(p1, p2, 1);
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
                transform.Translate(Vector3.left * FileReader.GridSpacing);
            if (vector.x > 0)
                transform.Translate(Vector3.right * FileReader.GridSpacing);
            if (vector.z < 0)
                transform.Translate(Vector3.back * FileReader.GridSpacing);
            if (vector.z > 0)
                transform.Translate(Vector3.forward * FileReader.GridSpacing);
        }
        jumping = false;
    }
    private void MoveForward()
    {
        if (jumping)
        {
            JumpMovement(Vector3.forward);
            return;
        }
        if (!CheckMove(Vector3.forward))
            StartCoroutine(Move(transform.position, transform.position + (Vector3.forward * FileReader.GridSpacing), InputRate));
    }
    private void MoveBackward()
    {
        if (jumping)
        {
            JumpMovement(Vector3.back);
            return;
        }        
        if (!CheckMove(Vector3.back))
            StartCoroutine(Move(transform.position, transform.position + (Vector3.back * FileReader.GridSpacing), InputRate));       
    }
    private void MoveLeft()
    {
        if (jumping)
        {
            JumpMovement(Vector3.left);
            return;
        }
        if (!CheckMove(Vector3.left))
            StartCoroutine(Move(transform.position, transform.position + (Vector3.left * FileReader.GridSpacing), InputRate));
    }
    private void MoveRight()
    {
        if (jumping)
        {
            JumpMovement(Vector3.right);
            return;
        }
        if (!CheckMove(Vector3.right))
            StartCoroutine(Move(transform.position, transform.position + (Vector3.right * FileReader.GridSpacing), InputRate));
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
