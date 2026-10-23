using Core.Player;
using UnityEngine;

namespace UnityAdapters.Interaction
{
    public class LimbDummy : MonoBehaviour
    {
        [SerializeField] private LimbType _limb = LimbType.RightLeg;
        private void Awake()
        {
            if (transform.childCount > 0)
                return;
            bool leg = _limb == LimbType.LeftLeg || _limb == LimbType.RightLeg;
            Color skin = new Color(0.72f, 0.55f, 0.45f);
            if (leg)
            {
                Part("Thigh", PrimitiveType.Capsule, new Vector3(0f, 0.55f, 0f), new Vector3(0.22f, 0.28f, 0.22f), skin);
                Part("Shin", PrimitiveType.Capsule, new Vector3(0f, 0.12f, 0.02f), new Vector3(0.16f, 0.24f, 0.16f), skin);
                Part("Foot", PrimitiveType.Cube, new Vector3(0f, -0.18f, 0.12f), new Vector3(0.16f, 0.08f, 0.28f), skin);
            }
            else
            {
                Part("Upper", PrimitiveType.Capsule, new Vector3(0f, 0.34f, 0f), new Vector3(0.14f, 0.2f, 0.14f), skin);
                Part("Forearm", PrimitiveType.Capsule, new Vector3(0f, 0.02f, 0f), new Vector3(0.11f, 0.18f, 0.11f), skin);
                Part("Hand", PrimitiveType.Cube, new Vector3(0f, -0.22f, 0f), new Vector3(0.12f, 0.08f, 0.16f), skin);
            }
            GameObject highlight = new GameObject("Highlight");
            highlight.transform.SetParent(transform, false);
            highlight.transform.localScale = Vector3.one * 1.08f;
            int count = transform.childCount;
            for (int i = 0; i < count; i++)
            {
                Transform child = transform.GetChild(i);
                if (child == highlight.transform)
                    continue;
                GameObject copy = Instantiate(child.gameObject, highlight.transform);
                copy.transform.localPosition = child.localPosition;
                copy.transform.localRotation = child.localRotation;
                copy.transform.localScale = child.localScale;
                Destroy(copy.GetComponent<Collider>());
                MeshRenderer renderer = copy.GetComponent<MeshRenderer>();
                Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
                if (shader == null)
                    shader = Shader.Find("Unlit/Color");
                renderer.sharedMaterial = new Material(shader) { color = new Color(1f, 0.85f, 0.2f) };
            }
            highlight.SetActive(false);
            CapsuleCollider body = gameObject.AddComponent<CapsuleCollider>();
            body.height = leg ? 1.15f : 0.85f;
            body.radius = 0.16f;
            body.center = new Vector3(0f, leg ? 0.25f : 0.08f, 0f);
            InteractableHighlight marker = GetComponent<InteractableHighlight>();
            if (marker == null)
                marker = gameObject.AddComponent<InteractableHighlight>();
            marker.Bind(highlight);
            int layer = LayerMask.NameToLayer("Interactable");
            if (layer >= 0)
                gameObject.layer = layer;
        }
        private void Part(string name, PrimitiveType type, Vector3 position, Vector3 scale, Color color)
        {
            GameObject part = GameObject.CreatePrimitive(type);
            part.name = name;
            part.transform.SetParent(transform, false);
            part.transform.localPosition = position;
            part.transform.localScale = scale;
            Destroy(part.GetComponent<Collider>());
            part.GetComponent<MeshRenderer>().material.color = color;
        }
    }
}