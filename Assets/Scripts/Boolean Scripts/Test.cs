using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Parabox.CSG.Test
{
    public class Test : MonoBehaviour
    {
        //[SerializeField]
        private GameObject heldObj;
        private GameObject collidingObj;

        GameObject left, right, composite;

        public float pickUpRange = 5;
        public float moveForce = 250;
        public Transform holdParent;

        public void SetObjects()
        {
            if (composite) Destroy(composite);
            if (left) Destroy(left);
            if (right) Destroy(right);

            GameObject go1 = Instantiate(heldObj);
            GameObject go2 = Instantiate(collidingObj);

            left = Instantiate(go1.transform.gameObject);
            right = Instantiate(go2.transform.gameObject);

            //GenerateBarycentric(left);
            //GenerateBarycentric(right);
        }

        enum BoolOp
        {
            Union, 
            SubtractLR
        }

        public void Union()
        {
            DoBooleanOperation(BoolOp.Union);
        }

        public void SubtractionLR()
        {
            DoBooleanOperation(BoolOp.SubtractLR);
        }

        void DoBooleanOperation(BoolOp operation)
        {
            Model result;

            switch (operation)
            {
                case BoolOp.Union:
                    result = CSG.Union(left, right);
                    break;

                default:
                    result = CSG.Subtract(left, right);
                    break;
            }
            composite = new GameObject();
            composite.AddComponent<MeshFilter>().sharedMesh = result.mesh;
            composite.AddComponent<MeshRenderer>().sharedMaterials = result.materials.ToArray();
            GenerateBarycentric(composite);
            Destroy(left);
            Destroy(right);
        }

        void GenerateBarycentric(GameObject go)
        {
            Mesh m = go.GetComponent<MeshFilter>().sharedMesh;

            if (m == null) return;

            int[] tris = m.triangles;
            int triangleCount = tris.Length;

            Vector3[] mesh_vertices = m.vertices;
            Vector3[] mesh_normals = m.normals;
            Vector2[] mesh_uv = m.uv;

            Vector3[] vertices = new Vector3[triangleCount];
            Vector3[] normals = new Vector3[triangleCount];
            Vector2[] uv = new Vector2[triangleCount];
            Color[] colors = new Color[triangleCount];

            for (int i = 0; i < triangleCount; i++)
            {
                vertices[i] = mesh_vertices[tris[i]];
                normals[i] = mesh_normals[tris[i]];
                uv[i] = mesh_uv[tris[i]];

                colors[i] = i % 3 == 0 ? new Color(1, 0, 0, 0) : (i % 3) == 1 ? new Color(0, 1, 0, 0) : new Color(0, 0, 1, 0);

                tris[i] = i;
            }

            Mesh wireframeMesh = new Mesh();

            wireframeMesh.Clear();
            wireframeMesh.vertices = vertices;
            wireframeMesh.triangles = tris;
            wireframeMesh.normals = normals;
            wireframeMesh.colors = colors;
            wireframeMesh.uv = uv;

            go.GetComponent<MeshFilter>().sharedMesh = wireframeMesh;
        }




        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            
            print(left);
            print(right);
            print(composite);


            HitObject();

            if (Input.GetKeyDown(KeyCode.Q))
            {
                Union();
            }
            

        }


        void HitObject()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {

                SetObjects();
                if (heldObj == null)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange))
                    {
                        PickupObject(hit.transform.gameObject);
                        print(hit.transform.gameObject);
                    }
                }
                else
                {
                    DropObject();
                }
            }
            if (heldObj != null)
            {
                MoveObject();
            }
        }


        void MoveObject()
        {
            if (Vector3.Distance(heldObj.transform.position, holdParent.position) > 0.1f)
            {
                Vector3 moveDirection = (holdParent.position - heldObj.transform.position);
                heldObj.GetComponent<Rigidbody>().AddForce(moveDirection * moveForce);
            }
        }

        void PickupObject(GameObject pickObj)
        {
            if (pickObj.GetComponent<Rigidbody>())
            {
                Rigidbody objRig = pickObj.GetComponent<Rigidbody>();
                objRig.useGravity = false;
                objRig.drag = 10;
                objRig.transform.parent = holdParent;
                heldObj = pickObj;
                pickObj.GetComponent<Collider>().enabled = false;
            }
        }

        void DropObject()
        {
            heldObj.GetComponent<Collider>().enabled = true;



            Rigidbody heldRig = heldObj.GetComponent<Rigidbody>();
            heldRig.useGravity = true;
            heldRig.drag = 1;

            heldObj.transform.parent = null;
            heldObj = null;
        }

        void OnTriggerEnter(Collision collision)
        {
            if (collision.collider)
            {
                heldObj.GetComponent<Rigidbody>().useGravity = false;
                //heldObj.GetComponent<BoxCollider>().enabled = false;
            }
        }


    }
}