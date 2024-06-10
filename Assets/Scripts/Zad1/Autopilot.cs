using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Autopilot : MonoBehaviour
{
    public bool On;

    [Header("Player/Enemy")]
    [SerializeField] GameObject enemy;
    [SerializeField] GameObject player;

    [Header("Distance")]
    public float distanceTEST;

    public float distance;

    [Header("Dot:  Value between -1 and 1")]
    public float dotTEST;
    public float dot;

    [Header("Angle")]
    public float angleTEST;
    public float angle;

    [Header("CrossProduct")]
    public Vector3 CrossProductTEST;
    public Vector3 CrossProduct;

    [Header("Other")]
    [SerializeField] int clockwise;


    Vector2 direction;
    Vector2 yaxis;

    void Update()
    {
        if(On)
        AutoMovePlayer();
    }



    private float GetDistance()
    {
        float number1 = player.transform.position.x - enemy.transform.position.x;
        float number2 = player.transform.position.y - enemy.transform.position.y;
        distance = (float)Math.Sqrt(number1 * number1 + number2* number2);

        distanceTEST = Vector2.Distance(player.transform.position, enemy.transform.position);

        return distance;
    }

    public float GetDot()
    {
        direction = (player.transform.position - enemy.transform.position).normalized;
        yaxis = player.transform.up;
        dotTEST = Vector2.Dot(yaxis, direction);

        dot = (yaxis.x * direction.x) + (yaxis.y * direction.y);
        return dot;
    }

    private float GetAngle()
    {
       // direction = (enemy.transform.position - player.transform.position).normalized;

        angleTEST = Vector2.SignedAngle(yaxis, direction);

        float playerMagnitude = yaxis.magnitude;
        float enemyMagnitude = direction.magnitude;

        if (playerMagnitude <= 0f || enemyMagnitude <= 0f)
        {
            return 0f;
        }

        float angleRad = dot / (playerMagnitude * enemyMagnitude);
        angle = Mathf.Rad2Deg * Mathf.Acos(Mathf.Clamp(angleRad, -1f, 1f));
        //angle = Mathf.Rad2Deg * Mathf.Clamp(angleRad, -1f, 1f);
        return angle;

    }
    private Vector3 GetCrossProduct()
    {
        CrossProductTEST = Vector3.Cross(yaxis, direction);

        Vector3 lhs = yaxis;
        Vector3 rhs = direction;
        CrossProduct = new Vector3(lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z, lhs.x * rhs.y - lhs.y * rhs.x);



        return CrossProduct;

    }

    private void OnDrawGizmos()
    {
        if (On)
        {
        //distance
        Gizmos.color = Color.red;
        Gizmos.DrawLine(player.transform.position, enemy.transform.position);

        //tekst distance
        Vector2 labelPosition = (player.transform.position + enemy.transform.position) / 2f;
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        Handles.Label(labelPosition, $"Dystans: {distance}", style);

        float radius = 2f;
        // k¹t
        Handles.DrawWireArc(player.transform.position, Vector3.forward, Vector3.down, angle, radius);
        Gizmos.DrawLine(player.transform.position, new Vector2(player.transform.position.x, player.transform.position.y - radius));
        Handles.Label(new Vector2(player.transform.position.x, player.transform.position.y - radius/2), $"Angle: {angleTEST}", style);


        //Cross product

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(CrossProduct, radius / 8);
        Gizmos.DrawLine(CrossProduct,player.transform.position);
        Gizmos.DrawLine(CrossProduct, enemy.transform.position);

            Gizmos.color= Color.black;
            Gizmos.DrawLine(player.transform.position, Vector2.up);

        }




    }

    private void AutoMovePlayer()
    {
        Vector2 direction = (enemy.transform.position - player.transform.position);

        GetDistance();
        GetDot();
        GetAngle();
        GetCrossProduct();

        clockwise = 1;
        if (CrossProduct.z < 0)
        {
            clockwise = -1;
        }

        player.transform.Rotate(0, 0, angle * clockwise);
        player.GetComponent<Rigidbody2D>().velocity = direction.normalized;
    }


}
