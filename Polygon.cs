//https://bitbucket.org/UnityUIExtensions/unity-ui-extensions
//https://github.com/CiaccoDavide/Unity-UI-Polygon
namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/Polygon", 51)]
    public class Polygon : MaskableGraphic
    {
        [SerializeField]
        Texture _texture;

        public bool _fillCenter = true;
        public float _thickness = 5;

        public int _sides = 3;

        [Range(0, 360)]
        public float _rotation = 0;

        [Range(0, 1)]
        public float[] _verticesDistances;

        public override Texture mainTexture
        {
            get => _texture == null ? s_WhiteTexture : _texture;
        }

        public Texture texture
        {
            get => _texture;
            set
            {
                if (_texture == value) return;
                _texture = value;
                SetAllDirty();
            }
        }

        protected UIVertex[] SetVbo(Vector2[] vertices, Vector2[] uvs)
        {
            UIVertex[] vbo = new UIVertex[4];
            for (int i = 0; i < vertices.Length; i++)
            {
                var vert = UIVertex.simpleVert;
                vert.color = color;
                vert.position = vertices[i];
                vert.uv0 = uvs[i];
                vbo[i] = vert;
            }
            return vbo;
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            Vector2 prevX = Vector2.zero;
            Vector2 prevY = Vector2.zero;

            Vector2 uv0 = new Vector2(0, 0);
            Vector2 uv1 = new Vector2(0, 1);
            Vector2 uv2 = new Vector2(1, 1);
            Vector2 uv3 = new Vector2(1, 0);

            Vector2 pos0;
            Vector2 pos1;
            Vector2 pos2;
            Vector2 pos3;

            float degrees = 360f / _sides;
            int vertices = _sides + 1;

            if (_verticesDistances == null || _verticesDistances.Length != vertices)
            {
                _verticesDistances = new float[vertices];
                for (int i = 0; i < vertices - 1; i++) _verticesDistances[i] = 1;
            }

            // last vertex is also the first!

            _verticesDistances[vertices - 1] = _verticesDistances[0];

            for (int i = 0; i < vertices; i++)
            {
                float outer = -rectTransform.pivot.x * rectTransform.rect.width * _verticesDistances[i];
                float inner = -rectTransform.pivot.x * rectTransform.rect.width * _verticesDistances[i] + _thickness;

                float rad = Mathf.Deg2Rad * (i * degrees + _rotation);
                float c = Mathf.Cos(rad);
                float s = Mathf.Sin(rad);

                uv0 = new Vector2(0, 1);
                uv1 = new Vector2(1, 1);
                uv2 = new Vector2(1, 0);
                uv3 = new Vector2(0, 0);

                pos0 = prevX;
                pos1 = new Vector2(outer * c, outer * s);

                if (_fillCenter)
                {
                    pos2 = Vector2.zero;
                    pos3 = Vector2.zero;
                }
                else
                {
                    pos2 = new Vector2(inner * c, inner * s);
                    pos3 = prevY;
                }

                prevX = pos1;
                prevY = pos2;

                UIVertex[] verts = SetVbo(new[] { pos0, pos1, pos2, pos3 }, new[] { uv0, uv1, uv2, uv3 });
                vh.AddUIVertexQuad(verts);
            }
        }
    }
}
