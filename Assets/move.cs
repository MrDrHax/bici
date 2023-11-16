using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour
{
    public GameObject main;
    public GameObject[] wheels;

    public Transform referencePosition;

    public Mesh mainMesh;
    public Mesh[] wheelMeshes; 

    public Vector3 moveDirection = Vector3.forward;
    public float rotationAmount = 0.2f;
    public float wheelRotationAmount = 0.5f;

    float wheelAngle = 0;

    Vector3[] mainVertices;
    Vector3[][] wheelVertices;

    Vector3[] initialMainVertices;
    Vector3[][] initialWheelVertices;

    public Vector3[] wheelOffsets;

    // Start is called before the first frame update
    void Start()
    {
        mainMesh = main.GetComponent<MeshFilter>().mesh;

        wheelMeshes = new Mesh[wheels.Length];

        for (int i = 0; i < wheels.Length; i++)
        {
            wheelMeshes[i] = wheels[i].GetComponent<MeshFilter>().mesh;
        }

        // get the vertices of the main body
        mainVertices = mainMesh.vertices.Clone() as Vector3[];
        initialMainVertices = mainMesh.vertices.Clone() as Vector3[];

        wheelVertices = new Vector3[wheels.Length][];
        initialWheelVertices = new Vector3[wheels.Length][];
        for (int i = 0; i < wheels.Length; i++)
        {
            wheelVertices[i] = wheelMeshes[i].vertices.Clone() as Vector3[];
            initialWheelVertices[i] = wheelMeshes[i].vertices.Clone() as Vector3[];
        }

        // get the offsets of the wheels
        wheelOffsets = new Vector3[wheels.Length];
        for (int i = 0; i < wheels.Length; i++)
        {
            wheelOffsets[i] = wheels[i].transform.position - referencePosition.position;

            wheels[i].transform.position = referencePosition.position;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // move the main body and it's wheels forward

        Matrix4x4 translation = HW_Transforms.TranslationMat(moveDirection.x, moveDirection.y, moveDirection.z);
        Matrix4x4 rotation = HW_Transforms.RotateMat(rotationAmount, AXIS.Y);
        Matrix4x4 wheelRotationMat = HW_Transforms.RotateMat(wheelAngle, AXIS.X);

        Matrix4x4 carTransform = translation * rotation;

        // move the main body
        for (int i = 0; i < mainMesh.vertexCount; i++)
        {
            Vector4 temp = new Vector4(initialMainVertices[i].x, initialMainVertices[i].y, initialMainVertices[i].z, 1);
            mainVertices[i] = carTransform * temp;
        }

        // move the wheels
        for (int i = 0; i < wheelVertices.Length; i++)
        {
            Matrix4x4 wheelTranslation = HW_Transforms.TranslationMat(wheelOffsets[i].x, wheelOffsets[i].y, wheelOffsets[i].z);

            for (int j = 0; j < wheelVertices[i].Length; j++)
            {

                Vector4 temp = new Vector4(initialWheelVertices[i][j].x, initialWheelVertices[i][j].y, initialWheelVertices[i][j].z, 1);
                wheelVertices[i][j] = carTransform * wheelTranslation * wheelRotationMat * temp;
            }
        }

        // apply the new vertices to the meshes

        mainMesh.vertices = mainVertices;
        mainMesh.RecalculateNormals();

        for (int i = 0; i < wheelMeshes.Length; i++)
        {
            wheelMeshes[i].vertices = wheelVertices[i];
            wheelMeshes[i].RecalculateNormals();
        }

        wheelAngle += wheelRotationAmount;

        if (wheelAngle >= 360)
        {
            wheelAngle -= 360;
        }

        if (wheelAngle < 0)
        {
            wheelAngle += 360;
        }
    }
}
