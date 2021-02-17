using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace My3DGame
{
    class Basic3DObjects
    {
        public class Obj3D
        {
            public int start_index;         // where it is in the index buffer
            public int triangle_count;      // how many triangles
            public Rectangle source_rect;   // section within texture to sample from
            public Texture2D tex;           // object texture
            public Vector3 rot;             // optional rotation
            public Vector3 pos;             // position
            public Matrix transform;        // world transform matrix (scale * rot * position_translation)

            // If position/rotation changes:
            public void UpdateTransform()
            {
                if (rot == Vector3.Zero) transform = Matrix.CreateTranslation(pos);
                else transform = Matrix.CreateFromYawPitchRoll(rot.Y, rot.X, rot.Z) * Matrix.CreateTranslation(pos);
            }

            protected void Init(Vector3 Pos, string file)
            {
                start_index = ibuf_start;
                pos = Pos; transform = Matrix.CreateTranslation(Pos);
                tex = LoadTexture(file);
                if ((source_rect.Width < 1) || (source_rect.Height < 1)) source_rect = new Rectangle(0, 0, tex.Width, tex.Height);
            }

            protected void GetUVCoords(ref float u1, ref float v1, ref float u2, ref float v2)
            {
                u1 = source_rect.X / (float)tex.Width;
                v1 = source_rect.Y / (float)tex.Height;
                u2 = (source_rect.X + source_rect.Width) / (float)tex.Width;
                v2 = (source_rect.Y + source_rect.Height) / (float)tex.Height;
            }

            public void AddQuad(Vector3 Pos, float width, float height, Vector3 rotation, string textureFile, Rectangle? sourceRect)
            {
                if (sourceRect.HasValue) source_rect = sourceRect.Value;
                Init(Pos, textureFile); rot = rotation; UpdateTransform();                      // setup inital transform matrix
                float u1 = 0, v1 = 0, u2 = 1, v2 = 1, hw = width / 2, hl = height / 2;          // uv's, half-width, half-length
                GetUVCoords(ref u1, ref v1, ref u2, ref v2);
                Vector3 normal = Vector3.Up;                                                    // initial normal (before rotated by transform)
                float y = Pos.Y, l = -hw, r = hw + Pos.X, n = -hl + Pos.Z, f = hl + Pos.Z;      // y-coord, left, right, near, far
                AddVertex(l, y, f, normal, u1, v1); AddVertex(r, y, f, normal, u2, v1); AddVertex(r, y, n, normal, u2, v2); // (left,y,far),(right,y,far),(right,y,near) ---> moves clockwise
                AddVertex(l, y, n, normal, u1, v2);
                AddTriangle(0, 1, 2); triangle_count++;                                         // clockwise order
                AddTriangle(2, 3, 0); triangle_count++;
                vertexBuffer.SetData<VertexPositionNormalTexture>(vbuf_start * vbytes, verts, 0, v_cnt, vbytes); vbuf_start = v_cnt; v_cnt = 0;
                indexBuffer.SetData<ushort>(ibuf_start * ibytes, indices, 0, i_cnt); ibuf_start = i_cnt; i_cnt = 0;
            }

            public void AddCube(Vector3 Pos, Vector3 size, Vector3 rotation, string textureFile, Rectangle? sourceRect)
            {
                if (sourceRect.HasValue) source_rect = sourceRect.Value;
                Init(Pos, textureFile); rot = rotation; UpdateTransform();                                              // setup inital transform matrix
                float u1 = 0, v1 = 0, u2 = 1, v2 = 1, hw = size.X / 2, hl = size.Z / 2, hh = size.Y / 2;                // uv's, half-width, half-length, half-height
                GetUVCoords(ref u1, ref v1, ref u2, ref v2);
                float t = Pos.Y - hh, b = Pos.Y + hh, l = Pos.X - hw, r = Pos.X + hw, n = Pos.Z - hl, f = Pos.Z + hl;   // y-coord, left, right, near, far
                Vector3 normal = Vector3.Up; AddVertex(l, t, f, normal, u1, v1); AddVertex(r, t, f, normal, u2, v1); AddVertex(r, t, n, normal, u2, v2); AddVertex(l, t, n, normal, u1, v2);    // (left,y,far),(right,y,far),(right,y,near) [clockwise]
                normal = Vector3.Right; AddVertex(r, b, f, normal, u1, v1); AddVertex(r, b, n, normal, u1, v2);
                normal = Vector3.Down; AddVertex(l, b, f, normal, u2, v1); AddVertex(l, b, n, normal, u2, v2);
                normal = Vector3.Backward; AddVertex(l, t, n, normal, u1, v1); AddVertex(r, t, n, normal, u2, v1); AddVertex(r, b, n, normal, u2, v2); AddVertex(l, b, n, normal, u1, v2);
                normal = Vector3.Forward; AddVertex(r, t, f, normal, u1, v1); AddVertex(l, t, f, normal, u2, v1); AddVertex(l, b, f, normal, u2, v2); AddVertex(r, b, f, normal, u1, v2);
                AddTriangle(0, 1, 2); triangle_count++; AddTriangle(2, 3, 0); triangle_count++;                         //clockwise order
                AddTriangle(2, 1, 4); triangle_count++; AddTriangle(4, 5, 2); triangle_count++;
                AddTriangle(5, 4, 6); triangle_count++; AddTriangle(6, 7, 5); triangle_count++;
                AddTriangle(7, 6, 0); triangle_count++; AddTriangle(0, 3, 7); triangle_count++;
                AddTriangle(8, 9, 10); triangle_count++; AddTriangle(10, 11, 8); triangle_count++;
                AddTriangle(12, 13, 14); triangle_count++; AddTriangle(14, 15, 12); triangle_count++;
                vertexBuffer.SetData<VertexPositionNormalTexture>(vbuf_start * vbytes, verts, 0, v_cnt, vbytes); vbuf_start = v_cnt; v_cnt = 0;
                indexBuffer.SetData<ushort>(ibuf_start * ibytes, indices, 0, i_cnt); ibuf_start = i_cnt; i_cnt = 0;
            }

            public void UpdateTransformQuaternion()
            {
                if (rot == Vector3.Zero) transform = Matrix.CreateTranslation(pos);
                else
                {
                    Quaternion qR = Quaternion.CreateFromYawPitchRoll(rot.Y, rot.X, rot.Z);
                    transform = Matrix.CreateFromQuaternion(qR) * Matrix.CreateTranslation(pos);
                }
            }
        }

        // Common
        GraphicsDevice gpu;
        // Light light; // <---- add later
        BasicEffect basic_effect;
        Vector3 upDirection;
        const int vbytes = sizeof(float) * 8, ibytes = sizeof(ushort); // each vertex contains 8 floats ( Position(1,2,3), Normal(4,5,6), texture(UV) coordinate (7,8) 

        // Geometry
        Matrix world;                                           // for fixed geometry (like landscape mesh)
        public List<Obj3D> objex;                               // list of 3D objects to render
        // static variables allow nested classes to access:
        static ContentManager Content;
        static IndexBuffer indexBuffer;                         // holds and controls transfer of indices to hardware
        static VertexBuffer vertexBuffer;                       // holds and controls transfer of vertices to hardware
        static ushort[] indices;                                // index list for index buffer
        static VertexPositionNormalTexture[] verts;             // vertex list for assembling vertex buffer
        static int i_cnt, ibuf_start = 0;                       // for making texture-sort id's, index-buffer's current starting position (for adding new objects)
        static int v_cnt, vbuf_start = 0;                       // v_cnt is for making current object, total_verts keeps track of how many were already added (and can be used as starting index when adding new ones)
        static Dictionary<string, Texture2D> textures;          // helps to store textures only once and get desired texture based on associated object's texture-file-name    

        // Constructor
        public Basic3DObjects(GraphicsDevice GPU, Vector3 UpDirection, ContentManager content) //, Light new_light)
        {
            gpu = GPU;
            world = Matrix.Identity;
            basic_effect = new BasicEffect(gpu);                // light = new_light <-- TO DO LATER [if you want instead of basic_effect)
            verts = new VertexPositionNormalTexture[65535];     //
            indices = new ushort[65535];                        // 64K indices for basic objects, you won't need more than this
            Content = content;
            upDirection = UpDirection;
            textures = new Dictionary<string, Texture2D>();
            vertexBuffer = new VertexBuffer(gpu, typeof(VertexPositionNormalTexture), 65535, BufferUsage.WriteOnly);
            indexBuffer = new IndexBuffer(gpu, typeof(ushort), 65535, BufferUsage.WriteOnly);
            objex = new List<Obj3D>();

            // Init
            basic_effect.Alpha = 1;                                            // Transparency
            basic_effect.LightingEnabled = true;                                // Enables lighting
            basic_effect.AmbientLightColor = new Vector3(0.1f, 0.2f, 0.3f);     // medium dark for dark parts of object
            basic_effect.DiffuseColor = new Vector3(0.94f, 0.94f, 0.94f);       // pretty bright for lit parts of object
            basic_effect.EnableDefaultLighting();                               // Lighting is on
            basic_effect.TextureEnabled = true;                                 // Enables textures
        }

        static void AddVertex(float x, float y, float z, Vector3 normal, float u, float v)
        {
            if ((vbuf_start + v_cnt) > 65530) { Console.WriteLine("Exceeded vertex buffer size"); return; }
            verts[v_cnt] = new VertexPositionNormalTexture(new Vector3(x, y, z), normal, new Vector2(u, v));
            v_cnt++;
        }

        static void AddTriangle(ushort a, ushort b, ushort c)
        {
            if ((ibuf_start + 3) > 65530) { Console.WriteLine("Exceeded index buffer size (may need UIint32 type)"); return; }
            ushort offset = (ushort)vbuf_start;
            a += offset; b += offset; c += offset;
            indices[i_cnt] = a; i_cnt++; indices[i_cnt] = b; i_cnt++; indices[i_cnt] = c; i_cnt++; // MUST CAST AGAIN due to math always converts to int32
        }

        static Texture2D LoadTexture(string name)
        {
            Texture2D texture;
            if(textures.TryGetValue(name, out texture) == true)
            {
                return texture; // if texture is already in list, just return the reference to texture in list
            }
            else
            {
                texture = Content.Load<Texture2D>(name);
                textures.Add(name, texture);
                return texture;
            }
        }

        // ADDING BASIC OBJECTS --------------------------
        // Basic Floor
        public void AddFloor(float width, float length, Vector3 mid_position, Vector3 rotation, string textureFile, Rectangle? sourceRect)
        {
            Obj3D obj = new Obj3D();
            obj.AddQuad(mid_position, width, length, rotation, textureFile, sourceRect);
            Console.WriteLine("adding floor with texture " + textureFile);
            objex.Add(obj);
            Console.WriteLine("Added!");
        }

        // Basic Cube
        public void AddCube(float width, float length, float height, Vector3 mid_position, Vector3 rotation, string textureFile, Rectangle? sourceRect)
        {
            Obj3D obj = new Obj3D();
            obj.AddCube(mid_position, new Vector3(width, height, length), rotation, textureFile, sourceRect);
            Console.WriteLine("adding cube with texture " + textureFile);
            objex.Add(obj);
            Console.WriteLine("Added!");
        }

        // Draw
        public void Draw(Camera cam)
        {
            gpu.SetVertexBuffer(vertexBuffer);
            gpu.Indices = indexBuffer;
            int obj_cnt = objex.Count, o;
            o = 0; while (o < obj_cnt)
            {
                Obj3D ob = objex[o];
                #region // Reminder_To_Set_Lighting_Draw_Params_Later for custom lighting class and effect
                // TO DO (later): 
                // if (DrawDepth)        light.SetDepthParams(ob.transform);           // for drawing to a depth shader
                // else if (DrawShadows) light.SetShadowParams(ob.transform, cam);     // for drawing shadows (using depth shader results) 
                // else                  light.SetDrawParams(ob.transform,cam,ob.tex); // regular drawing
                #endregion
                // Set shader paramaters:
                basic_effect.Texture = ob.tex;
                basic_effect.World = ob.transform;
                basic_effect.View = cam.view;
                basic_effect.Projection = cam.proj;
                basic_effect.FogEnabled = true;
                basic_effect.FogStart = 15;
                basic_effect.FogEnd = 500;
                basic_effect.FogColor = new Vector3(0f, 0f, 0f);
                foreach (EffectPass pass in basic_effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    gpu.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, ob.start_index, ob.triangle_count); o++;
                }

            }
        }
    }
}
