using System;
using UnityEngine;

namespace CarpenterSim.Timber
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(BoxCollider))]
    [RequireComponent(typeof(Rigidbody))]
    public sealed class TimberPiece : MonoBehaviour
    {
        public const float MillimetresPerMetre = 1000f;
        public const float DefaultDensityKgPerCubicMetre = 450f;

        [SerializeField, Min(float.Epsilon)] private float widthMm = 48f;
        [SerializeField, Min(float.Epsilon)] private float heightMm = 98f;
        [SerializeField, Min(float.Epsilon)] private float lengthMm = 4800f;
        [SerializeField, Min(float.Epsilon)] private float densityKgPerCubicMetre = DefaultDensityKgPerCubicMetre;

        public float WidthMm => widthMm;
        public float HeightMm => heightMm;
        public float LengthMm => lengthMm;
        public float DensityKgPerCubicMetre => densityKgPerCubicMetre;

        public Vector3 DimensionsMetres => new Vector3(
            widthMm / MillimetresPerMetre,
            heightMm / MillimetresPerMetre,
            lengthMm / MillimetresPerMetre);

        public float VolumeCubicMetres
        {
            get
            {
                Vector3 dimensions = DimensionsMetres;
                return dimensions.x * dimensions.y * dimensions.z;
            }
        }

        public float ExpectedMassKg => VolumeCubicMetres * densityKgPerCubicMetre;

        public void Configure(
            float newWidthMm,
            float newHeightMm,
            float newLengthMm,
            float newDensityKgPerCubicMetre = DefaultDensityKgPerCubicMetre)
        {
            ValidatePositive(newWidthMm, nameof(newWidthMm));
            ValidatePositive(newHeightMm, nameof(newHeightMm));
            ValidatePositive(newLengthMm, nameof(newLengthMm));
            ValidatePositive(newDensityKgPerCubicMetre, nameof(newDensityKgPerCubicMetre));

            widthMm = newWidthMm;
            heightMm = newHeightMm;
            lengthMm = newLengthMm;
            densityKgPerCubicMetre = newDensityKgPerCubicMetre;
            RebuildGeometry();
        }

        public void RebuildGeometry()
        {
            Vector3 dimensions = DimensionsMetres;
            Vector3 half = dimensions * 0.5f;

            MeshFilter meshFilter = GetComponent<MeshFilter>();
            Mesh mesh = meshFilter.sharedMesh;
            if (mesh == null || mesh.name != "TimberPiece Mesh")
            {
                mesh = new Mesh { name = "TimberPiece Mesh" };
                meshFilter.sharedMesh = mesh;
            }
            else
            {
                mesh.Clear();
            }

            mesh.vertices = new[]
            {
                new Vector3(-half.x, -half.y, -half.z),
                new Vector3( half.x, -half.y, -half.z),
                new Vector3( half.x,  half.y, -half.z),
                new Vector3(-half.x,  half.y, -half.z),
                new Vector3(-half.x, -half.y,  half.z),
                new Vector3( half.x, -half.y,  half.z),
                new Vector3( half.x,  half.y,  half.z),
                new Vector3(-half.x,  half.y,  half.z)
            };

            mesh.triangles = new[]
            {
                0, 2, 1, 0, 3, 2,
                4, 5, 6, 4, 6, 7,
                0, 4, 7, 0, 7, 3,
                1, 2, 6, 1, 6, 5,
                0, 1, 5, 0, 5, 4,
                3, 7, 6, 3, 6, 2
            };
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            transform.localScale = Vector3.one;

            BoxCollider boxCollider = GetComponent<BoxCollider>();
            boxCollider.center = Vector3.zero;
            boxCollider.size = dimensions;

            Rigidbody rigidbodyComponent = GetComponent<Rigidbody>();
            rigidbodyComponent.mass = ExpectedMassKg;
        }

        private void Awake()
        {
            RebuildGeometry();
        }

        private void OnValidate()
        {
            widthMm = Mathf.Max(float.Epsilon, widthMm);
            heightMm = Mathf.Max(float.Epsilon, heightMm);
            lengthMm = Mathf.Max(float.Epsilon, lengthMm);
            densityKgPerCubicMetre = Mathf.Max(float.Epsilon, densityKgPerCubicMetre);
            RebuildGeometry();
        }

        private static void ValidatePositive(float value, string parameterName)
        {
            if (float.IsNaN(value) || float.IsInfinity(value) || value <= 0f)
            {
                throw new ArgumentOutOfRangeException(parameterName, value, "Value must be finite and greater than zero.");
            }
        }
    }
}
