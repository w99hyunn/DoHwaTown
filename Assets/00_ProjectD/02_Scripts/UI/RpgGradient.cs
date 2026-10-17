using UnityEngine;
using UnityEngine.UI;

namespace DoHwa.UI
{
    public sealed class RpgGradient : BaseMeshEffect
    {
        [SerializeField] private Color top=Color.white;
        [SerializeField] private Color bottom=Color.gray;
        public void SetColors(Color upper,Color lower) { top=upper; bottom=lower; graphic.SetVerticesDirty(); }
        public override void ModifyMesh(VertexHelper mesh)
        {
            if(!IsActive()) return;
            var rect=graphic.rectTransform.rect;
            var vertex=new UIVertex();
            for(int i=0;i<mesh.currentVertCount;i++)
            {
                mesh.PopulateUIVertex(ref vertex,i);
                vertex.color=(Color)vertex.color*Color.Lerp(bottom,top,Mathf.InverseLerp(rect.yMin,rect.yMax,vertex.position.y));
                mesh.SetUIVertex(vertex,i);
            }
        }
    }
}
