using UnityEngine;

public class Player_CheckGround : MonoBehaviour
{
    [SerializeField][Header("足場を判定")] bool checkPlatform = true;
    [SerializeField][Header("足元の判定か")] bool isFoot = true;

    readonly string groundTag = "Map";
    readonly string waterTag = "Map_Water";
	readonly string wallTag = "Map_wall";
    //readonly string platformTag = "Map_platform";
    //readonly string movePlatTag = "Map_movePlat";
    //readonly string fallPlatTag = "Map_fallPlat";
    bool isGroundEnter, isGroundStay, isGroundExit;
    bool isWaterEnter, isWaterStay, isWaterExit;
	bool isWallEnter, isWallStay, isWallExit;

    //3つの判定をまとめる
    public bool IsGround()
    {
        bool isGround;
        if (isGroundEnter || isGroundStay) isGround = true;
        else if (isGroundExit) isGround = false;
        else isGround = false; //省略可　重かったら消す

        isGroundEnter = false;
        isGroundStay = false;
        isGroundExit = false;
        return isGround;
    }
    public bool IsWall()
    {
        bool isWall;
        if (isWallEnter || isWallStay) isWall = true;
        else if (isWallExit) isWall = false;
        else isWall = false;

        isWallEnter = false;
        isWallStay = false;
        isWallExit = false;
        return isWall;
    }
    public bool IsWater()
	{
		bool isWater;
		if (isWaterEnter || isWaterStay) isWater = true;
		else if (isWaterExit) isWater = false;
		else isWater = false;

		isWaterEnter = false;
		isWaterStay = false;
		isWaterExit = false;
		return isWater;
	}

	//接地判定
	void OnTriggerEnter2D(Collider2D collision)
    {
        /*
        bool isPlatCollision =
            collision.gameObject.CompareTag(platformTag) ||
            collision.gameObject.CompareTag(movePlatTag) ||
            collision.gameObject.CompareTag(fallPlatTag);
        */

        if (collision.gameObject.CompareTag(groundTag)) isGroundEnter = true;
        //else if (checkPlatform && isPlatCollision) isGroundEnter = true;

        if (isFoot)
        {
            //if (collision.gameObject.CompareTag(groundTag)) GManager.Instance.SetSEScenePath("Grass/");
            //else if (checkPlatform && isPlatCollision) GManager.Instance.SetSEScenePath("Wood/");
        }

        /*
        if (collision.gameObject.CompareTag(wallTag))
        {
            isWallEnter = true;
            //GManager.Instance.SetSEScenePath("Grass/");
        }
        */

        if (collision.gameObject.CompareTag(waterTag))
        {
            isWaterEnter = true;
        }
	}
    void OnTriggerStay2D(Collider2D collision)
    {
        /*
        bool isPlatCollision =
            collision.gameObject.CompareTag(platformTag) ||
            collision.gameObject.CompareTag(movePlatTag) ||
            collision.gameObject.CompareTag(fallPlatTag);
        */

        if (collision.gameObject.CompareTag(groundTag)) isGroundStay = true;
        //else if (checkPlatform && isPlatCollision) isGroundStay = true;

        if (isFoot)
        {
            //if (collision.gameObject.CompareTag(groundTag)) GManager.Instance.SetSEScenePath("Grass/");
            //else if (checkPlatform && isPlatCollision) GManager.Instance.SetSEScenePath("Wood/");
        }

        /*
        if (collision.gameObject.CompareTag(wallTag))
        {
            isWallStay = true;
            //GManager.Instance.SetSEScenePath("Grass/");
        }
        */

        if (collision.gameObject.CompareTag(waterTag))
		{
			isWaterStay = true;
		}
	}
    void OnTriggerExit2D(Collider2D collision)
    {
        /*
        bool isPlatCollision =
            collision.gameObject.CompareTag(platformTag) ||
            collision.gameObject.CompareTag(movePlatTag) ||
            collision.gameObject.CompareTag(fallPlatTag);
        */

        if (collision.gameObject.CompareTag(groundTag)) isGroundExit = true;
        //else if (checkPlatform && isPlatCollision) isGroundExit = true;

        //if (collision.gameObject.CompareTag(wallTag)) isWallExit = true;

        if (collision.gameObject.CompareTag(waterTag)) isWaterExit = true;

	}
}
