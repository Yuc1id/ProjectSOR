using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Main : MonoBehaviour
{

    [SerializeField] PlayerInput inputSys; //InputSystem Object

    Rigidbody2D rb = null; //物理演算
    InputAction moveKey, jumpKey;
    float moveKeyFloat;
    bool jumpKeyBool;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        //キー入力の取得
        var actionMap = inputSys.currentActionMap;
        moveKey = actionMap["Move"]; //1D
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
