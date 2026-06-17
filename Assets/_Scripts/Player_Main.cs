using UnityEngine;

public class Player_Main : MonoBehaviour
{

	[SerializeField] InputSystem_Actions inputSys; //InputSystem Object

	Rigidbody2D rb = null; //物理演算
	float moveKeyFloat;
	bool jumpKeyBool;

	private void Awake()
	{
		inputSys = new InputSystem_Actions();
	}
	private void OnEnable()
	{
		inputSys.Enable();
	}
	private void OnDisable()
	{
		inputSys.Disable();
	}

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();

	}

	void Update()
	{
		moveKeyFloat = inputSys.Player.Move.ReadValue<Vector2>().x;
	}
}
