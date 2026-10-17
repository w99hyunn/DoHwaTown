using UnityEngine;
using UnityEngine.UI;

namespace DoHwa.UI
{
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class RpgGlyph : MaskableGraphic
    {
        public enum Kind { Bag,Sword,Food,Material,Hammer,Journal,Map,Star,Arrow }
        [SerializeField] private Kind kind;
        public void SetKind(Kind value) { kind=value; SetVerticesDirty(); }
        protected override void OnPopulateMesh(VertexHelper mesh)
        {
            mesh.Clear();
            switch(kind)
            {
                case Kind.Bag:
                    Poly(mesh,new Vector2(-.63f,.35f),new Vector2(.63f,.35f),new Vector2(.72f,-.67f),new Vector2(-.72f,-.67f));
                    Path(mesh,.13f,new Vector2(-.31f,.39f),new Vector2(-.31f,.72f),new Vector2(.31f,.72f),new Vector2(.31f,.39f));
                    Line(mesh,new Vector2(-.81f,.3f),new Vector2(.81f,.3f),.18f);
                    break;
                case Kind.Sword:
                    Poly(mesh,new Vector2(-.13f,-.25f),new Vector2(-.28f,-.08f),new Vector2(.45f,.73f),new Vector2(.73f,.84f),new Vector2(.69f,.49f));
                    Line(mesh,new Vector2(-.5f,.04f),new Vector2(.07f,-.5f),.13f); Line(mesh,new Vector2(-.25f,-.27f),new Vector2(-.66f,-.7f),.19f); break;
                case Kind.Food:
                    Circle(mesh,new Vector2(-.25f,-.1f),.47f); Circle(mesh,new Vector2(.25f,-.1f),.47f);
                    Poly(mesh,new Vector2(-.2f,.47f),new Vector2(.1f,.84f),new Vector2(.65f,.7f),new Vector2(.18f,.45f)); break;
                case Kind.Material:
                    Path(mesh,.11f,new Vector2(-.72f,.28f),new Vector2(0,.68f),new Vector2(.72f,.28f),new Vector2(.72f,-.48f),new Vector2(0,-.85f),new Vector2(-.72f,-.48f),new Vector2(-.72f,.28f),new Vector2(0,-.05f),new Vector2(.72f,.28f));
                    Line(mesh,new Vector2(0,-.05f),new Vector2(0,-.85f),.1f); break;
                case Kind.Hammer:
                    Line(mesh,new Vector2(-.58f,-.72f),new Vector2(.23f,.36f),.24f);
                    Poly(mesh,new Vector2(-.29f,.58f),new Vector2(.03f,.87f),new Vector2(.76f,.25f),new Vector2(.43f,-.06f)); break;
                case Kind.Journal:
                    Path(mesh,.12f,new Vector2(.6f,-.72f),new Vector2(-.6f,-.72f),new Vector2(-.6f,.72f),new Vector2(.6f,.72f),new Vector2(.6f,-.72f));
                    Line(mesh,new Vector2(-.3f,.72f),new Vector2(-.3f,-.72f),.1f);
                    Line(mesh,new Vector2(-.06f,.3f),new Vector2(.35f,.3f),.1f); Line(mesh,new Vector2(-.06f,.02f),new Vector2(.35f,.02f),.1f); break;
                case Kind.Map:
                    Path(mesh,.1f,new Vector2(-.75f,-.55f),new Vector2(-.75f,.55f),new Vector2(-.25f,.8f),new Vector2(.25f,.55f),new Vector2(.75f,.8f),new Vector2(.75f,-.55f),new Vector2(.25f,-.8f),new Vector2(-.25f,-.55f),new Vector2(-.75f,-.8f),new Vector2(-.75f,-.55f));
                    Line(mesh,new Vector2(-.25f,.8f),new Vector2(-.25f,-.55f),.08f); Line(mesh,new Vector2(.25f,.55f),new Vector2(.25f,-.8f),.08f); break;
                case Kind.Star:
                    Poly(mesh,new Vector2(0,.9f),new Vector2(.23f,.24f),new Vector2(.9f,0),new Vector2(.23f,-.24f),new Vector2(0,-.9f),new Vector2(-.23f,-.24f),new Vector2(-.9f,0),new Vector2(-.23f,.24f)); break;
                case Kind.Arrow:
                    Poly(mesh,new Vector2(0,.85f),new Vector2(.65f,-.65f),new Vector2(0,-.38f),new Vector2(-.65f,-.65f)); break;
            }
        }
        private Vector2 Position(Vector2 value) => rectTransform.rect.center+value*Mathf.Min(rectTransform.rect.width,rectTransform.rect.height)*.5f;
        private void Poly(VertexHelper mesh,params Vector2[] points)
        {
            int start=mesh.currentVertCount; Vector2 center=Vector2.zero; foreach(var p in points) center+=p; center/=points.Length;
            mesh.AddVert(Position(center),color,Vector2.zero); foreach(var p in points) mesh.AddVert(Position(p),color,Vector2.zero);
            for(int i=0;i<points.Length;i++) mesh.AddTriangle(start,start+1+i,start+1+(i+1)%points.Length);
        }
        private void Circle(VertexHelper mesh,Vector2 center,float radius)
        { var points=new Vector2[24]; for(int i=0;i<24;i++) points[i]=center+new Vector2(Mathf.Cos(i*Mathf.PI/12),Mathf.Sin(i*Mathf.PI/12))*radius; Poly(mesh,points); }
        private void Path(VertexHelper mesh,float width,params Vector2[] points) { for(int i=1;i<points.Length;i++) Line(mesh,points[i-1],points[i],width); }
        private void Line(VertexHelper mesh,Vector2 a,Vector2 b,float width)
        { var n=new Vector2(a.y-b.y,b.x-a.x).normalized*width*.5f; Poly(mesh,a+n,b+n,b-n,a-n); }
    }
}
