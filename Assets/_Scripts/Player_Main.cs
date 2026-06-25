using UnityEngine;

public class Player_Main : MonoBehaviour
{
	//Constant
	[SerializeField] Player_CheckGround checkGround, checkHead;
	[SerializeField] float walkSpeed = 10;
	[SerializeField] AnimationCurve walkCurve; //移動速度表現
	[SerializeField] float gravity; //下向き＋
	[SerializeField] float jumpSpeed;
	[SerializeField] AnimationCurve jumpCurve; //ジャンプ速度表現
	[SerializeField] float jumpLimitTime; //ジャンプ上限時間
	[SerializeField] AnimationCurve objJumpCurve; //オブジェクトジャンプ表現
	//Objects
	InputSystem_Actions inputSys; //InputSystem Object
	Rigidbody2D rb = null; //physics

	//Player_CheckGround checkGround, checkHead;


	//Input
	float walkKeyFloat;
	bool jumpKeyBool;

	//Script Variable
	bool isGround = false; //CheckGround under
	bool isHead = false; //CheckGround above

	float beforeWalkSpeed = 0f;
	float walkTime; //for AnimationCurve

	float beforeJumpSpeed = 0f;
	float jumpTime; //for AnimationCurve

	bool isJump = false; //プレイヤーがジャンプ（上昇）中か
	bool isObjectJump = false; //プレイヤーがオブジェクト（ばねなど）によるジャンプ（上昇）中か
	float objectJumpHeight = 0f; //オブジェクトジャンプの高度
	//Animation Variable
	float animWalkSpeed = 0.0f;
	bool animIsJump = false;
	bool animIsWallJump = false;

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

	Vector2 GetSpeed_Jump(bool jKey)
	{
		//return Vector2.zero;

		const float maxFallSpeed = -10f;

		bool canTime = jumpLimitTime > jumpTime; //ジャンプが時間切れでないか
		float calcSpeed = beforeJumpSpeed;

		if (isHead) calcSpeed = 0.0f;

		if (isObjectJump)
		{
			if (canTime && !isHead) //上昇続行　AnimationCurve適用
			{
				calcSpeed = objectJumpHeight * objJumpCurve.Evaluate(jumpTime);
				jumpTime += Time.deltaTime;
				/* グラフ計算詳細
				各点での傾きなどから速度変化のグラフを手動で書く
				x秒で目的の高さまで上げる

				f(0) = 0 => a * 0**2 + b * 0 + c = 0 => c = 0
				f(x) = p => a * x**2 + b * x = p => a * x + b = p * (1/x)
				f'(x) = 0 => 2a * x + b = 0
				a * x = -p * (1/x) => a = -p * (1/x)**2, b = -2 * x * a = 2p * (1/x)

				a = -(1/x**2)p, b = (2/x)p
				f(t) = -(1/x**2)p * t**2 + (2/x)p * t
				f'(t) = -(2/x**2)p * t + (2/x)p
				f'(0) = (2/x)p, f'(x) = 0

				x = 0.5
				f'(0) = 4p, f'(x) = 0
				*/
			}
			else //落下開始　AnimationCurve無視
			{
				isObjectJump = false;
				jumpTime = 0.0f;
				calcSpeed = 0.0f;
			}
		}
		else if (isJump) //ジャンプ上昇中
		{

			if (jKey && canTime && !isHead) //上昇続行　AnimationCurve適用
			{
				jumpTime += Time.deltaTime;
				calcSpeed = jumpSpeed * jumpCurve.Evaluate(jumpTime);
			}
			else //落下開始　AnimationCurve無視
			{
				isJump = false;
				jumpTime = 0.0f;
				calcSpeed -= gravity; //慣性を残す
			}
		}
		else if (isGround) //地上
		{
			if (jKey) //上昇開始　AnimationCurve適用
			{
				isJump = true;
				jumpTime = 0.0f;
				calcSpeed = jumpSpeed * jumpCurve.Evaluate(jumpTime);
			}
			else //地上
			{
				calcSpeed = -1f; //下向きの速度を与えて正確に接地させる
			}
		}
		else //落下続行　AnimationCurve無視
		{
			calcSpeed -= gravity;
			if (calcSpeed < maxFallSpeed) calcSpeed = maxFallSpeed; //下限設定
		}
		//現フレームのステータスを保存
		beforeJumpSpeed = calcSpeed;

		animIsJump = isJump || calcSpeed > 0f;
		return new Vector2(0, calcSpeed);
	}

	//
	void OnEnable()
	{
		inputSys = new InputSystem_Actions();
		inputSys.Enable();
	}
	void OnDisable()
	{
		inputSys.Disable();
	}

	void Start()
	{
		//Load Object
		rb = GetComponent<Rigidbody2D>();
        //checkGround = transform.Find("Ground").gameObject.GetComponent<Player_CheckGround>();
        //checkHead = transform.Find("Head").gameObject.GetComponent<Player_CheckGround>();
    }

	void FixedUpdate()
	{
		//Load Input
		walkKeyFloat = inputSys.Player.Move.ReadValue<Vector2>().x;
		jumpKeyBool = inputSys.Player.Jump.IsPressed();
		//Debug.Log(walkKeyFloat);

		//Move
		rb.linearVelocity = GetSpeed_Walk(walkKeyFloat) + GetSpeed_Jump(jumpKeyBool);
	}
}
