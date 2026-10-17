using UnityEngine;
using UnityEngine.UI;

namespace DoHwa.UI
{
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class LoginLineIcon : MaskableGraphic
    {
        public enum Symbol { Language, Settings, Notice, Exit, Star, Close }
        [SerializeField] private Symbol symbol;

        public void SetSymbol(Symbol value)
        {
            symbol = value;
            SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper mesh)
        {
            mesh.Clear();
            switch (symbol)
            {
                case Symbol.Language:
                    Ring(mesh, Vector2.zero, new Vector2(0.74f, 0.74f), 0.045f);
                    Ring(mesh, Vector2.zero, new Vector2(0.35f, 0.74f), 0.04f);
                    Line(mesh, new Vector2(-0.7f, 0), new Vector2(0.7f, 0), 0.04f);
                    Line(mesh, new Vector2(-0.61f, 0.35f), new Vector2(0.61f, 0.35f), 0.04f);
                    Line(mesh, new Vector2(-0.61f, -0.35f), new Vector2(0.44f, -0.35f), 0.04f);
                    Line(mesh, new Vector2(0.21f, -0.64f), new Vector2(0.89f, -0.64f), 0.07f);
                    Line(mesh, new Vector2(0.69f, -0.45f), new Vector2(0.89f, -0.64f), 0.06f);
                    Line(mesh, new Vector2(0.69f, -0.83f), new Vector2(0.89f, -0.64f), 0.06f);
                    break;
                case Symbol.Settings:
                    Ring(mesh, Vector2.zero, Vector2.one * 0.57f, 0.14f);
                    Ring(mesh, Vector2.zero, Vector2.one * 0.23f, 0.045f);
                    for (var i = 0; i < 8; i++)
                    {
                        var direction = new Vector2(Mathf.Cos(i * Mathf.PI / 4), Mathf.Sin(i * Mathf.PI / 4));
                        Line(mesh, direction * 0.53f, direction * 0.76f, 0.17f);
                    }
                    break;
                case Symbol.Notice:
                    Ring(mesh, new Vector2(-0.08f, 0.07f), Vector2.one * 0.62f, 0.05f, -0.2f, 4.15f);
                    Line(mesh, new Vector2(-0.63f, -0.22f), new Vector2(-0.75f, -0.66f), 0.05f);
                    Line(mesh, new Vector2(-0.75f, -0.66f), new Vector2(-0.28f, -0.45f), 0.05f);
                    Line(mesh, new Vector2(-0.1f, 0.4f), new Vector2(-0.1f, 0.02f), 0.065f);
                    Ring(mesh, new Vector2(-0.1f, -0.19f), Vector2.one * 0.035f, 0.05f);
                    Line(mesh, new Vector2(0.12f, -0.51f), new Vector2(0.62f, -0.04f), 0.13f);
                    Line(mesh, new Vector2(0.06f, -0.64f), new Vector2(0.25f, -0.6f), 0.08f);
                    break;
                case Symbol.Exit:
                    Line(mesh, new Vector2(-0.61f, -0.69f), new Vector2(-0.61f, 0.69f), 0.06f);
                    Line(mesh, new Vector2(-0.61f, 0.69f), new Vector2(0.31f, 0.69f), 0.06f);
                    Line(mesh, new Vector2(-0.61f, -0.69f), new Vector2(0.31f, -0.69f), 0.06f);
                    Line(mesh, new Vector2(0.31f, 0.69f), new Vector2(0.31f, 0.4f), 0.06f);
                    Line(mesh, new Vector2(0.31f, -0.69f), new Vector2(0.31f, -0.4f), 0.06f);
                    Line(mesh, new Vector2(-0.23f, 0), new Vector2(0.84f, 0), 0.07f);
                    Line(mesh, new Vector2(0.48f, 0.35f), new Vector2(0.84f, 0), 0.07f);
                    Line(mesh, new Vector2(0.48f, -0.35f), new Vector2(0.84f, 0), 0.07f);
                    break;
                case Symbol.Close:
                    Line(mesh, new Vector2(-0.62f, -0.62f), new Vector2(0.62f, 0.62f), 0.08f);
                    Line(mesh, new Vector2(-0.62f, 0.62f), new Vector2(0.62f, -0.62f), 0.08f);
                    break;
                case Symbol.Star:
                    var points = new[] { new Vector2(0, 0.86f), new Vector2(0.23f, 0.23f), new Vector2(0.86f, 0), new Vector2(0.23f, -0.23f), new Vector2(0, -0.86f), new Vector2(-0.23f, -0.23f), new Vector2(-0.86f, 0), new Vector2(-0.23f, 0.23f) };
                    for (var i = 0; i < points.Length; i++) Line(mesh, points[i], points[(i + 1) % points.Length], 0.1f);
                    break;
            }
        }

        private void Ring(VertexHelper mesh, Vector2 center, Vector2 radius, float width, float start = 0, float arc = Mathf.PI * 2)
        {
            for (var i = 0; i < 64; i++)
            {
                var a = start + arc * i / 64;
                var b = start + arc * (i + 1) / 64;
                Line(mesh, center + new Vector2(Mathf.Cos(a) * radius.x, Mathf.Sin(a) * radius.y), center + new Vector2(Mathf.Cos(b) * radius.x, Mathf.Sin(b) * radius.y), width);
            }
        }

        private void Line(VertexHelper mesh, Vector2 from, Vector2 to, float width)
        {
            var extent = Mathf.Min(rectTransform.rect.width, rectTransform.rect.height) * 0.5f;
            var normal = new Vector2(-(to - from).y, (to - from).x).normalized * (width * 0.5f);
            var corners = new[] { from - normal, from + normal, to + normal, to - normal };
            var index = mesh.currentVertCount;
            foreach (var corner in corners) mesh.AddVert(rectTransform.rect.center + corner * extent, color, Vector2.zero);
            mesh.AddTriangle(index, index + 1, index + 2);
            mesh.AddTriangle(index, index + 2, index + 3);
        }
    }
}
