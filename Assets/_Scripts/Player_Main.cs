using UnityEngine;

public class Player_Main : MonoBehaviour
{
	//Constant
	[SerializeField] float moveSpeed = 10;
    [SerializeField] AnimationCurve walkCurve; //移動速度表現

    //Objects
    private InputSystem_Actions inputSys; //InputSystem Object
	private Rigidbody2D rb = null; //物理演算

	//Input
	float moveKeyFloat;
	bool jumpKeyBool;

	//Script Variable
    float beforeWalkSpeed = 0f; //直前の歩行の速度
    float walkTime; //AnimationCurve用

    //
    private Vector2 GetSpeed_Walk(float hKey)
	{
		return new Vector2(hKey * moveSpeed, 0);
	}

	//
	private void OnEnable()
	{
        inputSys = new InputSystem_Actions();
        inputSys.Enable();
	}
	private void OnDisable()
	{
		inputSys.Disable();
	}

	void Start()
	{
		//Load Object
		rb = GetComponent<Rigidbody2D>();
	}

	void FixedUpdate()
	{
		//Load Input
		moveKeyFloat = inputSys.Player.Move.ReadValue<Vector2>().x;
		//Debug.Log(moveKeyFloat);

		//Move
        rb.linearVelocity = new Vector2(moveKeyFloat * 10, 0);
    }
}
