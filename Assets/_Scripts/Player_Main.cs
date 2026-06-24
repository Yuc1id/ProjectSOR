using UnityEngine;

public class Player_Main : MonoBehaviour
{
	//Constant
	[SerializeField] float walkSpeed = 10;
	[SerializeField] AnimationCurve walkCurve; //移動速度表現

	//Objects
	InputSystem_Actions inputSys; //InputSystem Object
	Rigidbody2D rb = null; //physics

	//Input
	float walkKeyFloat;
	bool jumpKeyBool;

	//Script Variable
	float beforeWalkSpeed = 0f;
	float walkTime; //for AnimationCurve

	//Animation Variable
	float animWalkSpeed = 0.0f;

	/// <summary>
	///	歩行速度の計算（-speed以上speed以下）
	/// </summary>
	/// <param name="hKey">横移動キー</param>
	/// <returns>歩行速度(float)</returns>
	Vector2 GetSpeed_Walk(float hKey)
	{
		const int turnDecelRate = 5; //ターン（キーを切り替えた）時の減速最大フレーム数
		const int stopDecelRate = 15; //停止（キーを離した）時の減速最大フレーム数

		float calcSpeed = beforeWalkSpeed;

		//(hKey = -1, 0, 1)
		int bWS_sign = (int)(beforeWalkSpeed / Mathf.Abs(beforeWalkSpeed)); //-1, -2147483648(=-2**31=0), 1
		if (beforeWalkSpeed == 0) bWS_sign = 0;

		if (hKey != 0) //キーが押された
		{
			transform.localScale = new Vector3(hKey, 1, 1);
			//速度計算
			if (hKey * beforeWalkSpeed < 0) //ターン
			{
				calcSpeed += hKey * walkSpeed / turnDecelRate; //減速
				walkTime = 0.0f; //AnimationCurveを無視
				if (hKey * calcSpeed > 0) calcSpeed = 0.0f; //ターン終わり
			}
			else //直進(AnimationCurveにしたがって加速)
			{
				calcSpeed = hKey * walkSpeed * walkCurve.Evaluate(walkTime);
				walkTime += Time.deltaTime;
			}
		}
		else
		{
			calcSpeed -= bWS_sign * walkSpeed / stopDecelRate; //減速
			walkTime = 0.0f; //AnimationCurveを無視
			if (bWS_sign * calcSpeed < 0) calcSpeed = 0.0f; //減速終わり
		}
		//if (isWallAlt) //壁にめり込もうとする向き
		//{
		//	calcSpeed = 0f;
		//	walkTime = 0.0f;
		//}


		//最高速度(walkSpeed)に対する現在の速度(calcSpeed)の割合　アニメーション用
		animWalkSpeed = Mathf.Abs(calcSpeed / walkSpeed);
		//現フレームのステータスを保存
		beforeWalkSpeed = calcSpeed;

		return new Vector2(calcSpeed, 0);
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
		walkKeyFloat = inputSys.Player.Move.ReadValue<Vector2>().x;
		//Debug.Log(walkKeyFloat);

		//Move
		rb.linearVelocity = GetSpeed_Walk(walkKeyFloat);
	}
}
