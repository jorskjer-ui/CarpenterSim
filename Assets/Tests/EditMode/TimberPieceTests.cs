using CarpenterSim.Timber;
using NUnit.Framework;
using UnityEngine;

namespace CarpenterSim.Tests.EditMode
{
    public sealed class TimberPieceTests
    {
        private const float Tolerance = 0.000001f;
        private GameObject timberObject;
        private TimberPiece timber;

        [SetUp]
        public void SetUp()
        {
            timberObject = new GameObject("TimberPiece Test");
            timber = timberObject.AddComponent<TimberPiece>();
            timber.Configure(48f, 98f, 4800f, 450f);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(timberObject);
        }

        [Test]
        public void ReportsExactConstructionDimensionsInMillimetres()
        {
            Assert.That(timber.WidthMm, Is.EqualTo(48f));
            Assert.That(timber.HeightMm, Is.EqualTo(98f));
            Assert.That(timber.LengthMm, Is.EqualTo(4800f));
        }

        [Test]
        public void ConvertsDimensionsToMetresUsingCoordinateConvention()
        {
            AssertVector(timber.DimensionsMetres, new Vector3(0.048f, 0.098f, 4.8f));
        }

        [Test]
        public void MeshBoundsMatchPhysicalDimensions()
        {
            Mesh mesh = timber.GetComponent<MeshFilter>().sharedMesh;
            Assert.That(mesh, Is.Not.Null);
            AssertVector(mesh.bounds.size, new Vector3(0.048f, 0.098f, 4.8f));
        }

        [Test]
        public void BoxColliderMatchesPhysicalDimensions()
        {
            BoxCollider boxCollider = timber.GetComponent<BoxCollider>();
            AssertVector(boxCollider.size, new Vector3(0.048f, 0.098f, 4.8f));
            AssertVector(boxCollider.center, Vector3.zero);
        }

        [Test]
        public void TransformScaleRemainsOne()
        {
            AssertVector(timber.transform.localScale, Vector3.one);
        }

        [Test]
        public void VolumeMatchesConvertedDimensions()
        {
            float expectedVolume = 0.048f * 0.098f * 4.8f;
            Assert.That(timber.VolumeCubicMetres, Is.EqualTo(expectedVolume).Within(Tolerance));
        }

        [Test]
        public void RigidbodyMassMatchesVolumeTimesConfiguredDensity()
        {
            float expectedMass = (0.048f * 0.098f * 4.8f) * 450f;
            Assert.That(timber.DensityKgPerCubicMetre, Is.EqualTo(450f));
            Assert.That(timber.ExpectedMassKg, Is.EqualTo(expectedMass).Within(Tolerance));
            Assert.That(timber.GetComponent<Rigidbody>().mass, Is.EqualTo(expectedMass).Within(Tolerance));
        }

        private static void AssertVector(Vector3 actual, Vector3 expected)
        {
            Assert.That(actual.x, Is.EqualTo(expected.x).Within(Tolerance));
            Assert.That(actual.y, Is.EqualTo(expected.y).Within(Tolerance));
            Assert.That(actual.z, Is.EqualTo(expected.z).Within(Tolerance));
        }
    }
}
