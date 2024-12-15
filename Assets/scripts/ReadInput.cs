using UnityEngine;

public class ReadInput : MonoBehaviour
{
    private string input;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ReadInputString(string input)
    {
        this.input = input;
        Debug.Log(this.input);

    }

}
