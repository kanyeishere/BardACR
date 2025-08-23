using System;
using System.Numerics;
using ImGuiNET;

namespace Wotou.Common
{
    public static class CuteStopSign
    {
        public static void DrawDefault(Vector2 center, float size)
        {
            uint fill      = ImGui.GetColorU32(new Vector4(1.00f, 0.60f, 0.75f, 1f)); // 浅粉
            uint innerEdge = ImGui.GetColorU32(new Vector4(0.85f, 0.25f, 0.50f, 1f)); // 深粉内描边
            uint outerEdge = ImGui.GetColorU32(new Vector4(0, 0, 0, 1));              // 黑色外描边
            uint textCol   = ImGui.GetColorU32(new Vector4(1, 1, 1, 1));              // 白字
            uint faceCol   = ImGui.GetColorU32(new Vector4(0, 0, 0, 1));              // 黑色脸

            Draw(center, size, fill, innerEdge, outerEdge, textCol, faceCol);
        }

        public static void Draw(
            Vector2 center, float size,
            uint fill, uint innerBorder, uint outerBorder,
            uint textColor, uint faceColor)
        {
            var dl = ImGui.GetWindowDrawList();
            float r = size * 0.5f;

                // ===== 圆形主体 =====
            float outerThickness = Math.Max(2f, r * 0.18f);  // 黑外描边
            float innerThickness = Math.Max(1f, r * 0.08f);  // 深粉内描边
            // 填充（粉色圆）
            dl.AddCircleFilled(center, r, fill, 64); 
            // 内描边（深粉）
            dl.AddCircle(center, r, innerBorder, 64, innerThickness); 
            // 外描边（黑色）
            dl.AddCircle(center, r, outerBorder, 64, outerThickness);


            // ===== 笑脸（上半部，嘴短一点）=====
            float eyeSpread   = r * 0.32f;   // 眼睛左右偏移
            float mouthHalf   = r * 0.16f;   // 嘴半宽（短）
            float mouthDepth  = r * 0.12f;   // 嘴巴弧度
            float eyeR        = Math.Max(2f, r * 0.08f);

            float eyeY   = center.Y - r * 0.46f; // 眼睛更靠上
            float mouthY = eyeY + r * 0.06f;     // 嘴在眼睛稍下

            var leftEye  = new Vector2(center.X - eyeSpread, eyeY);
            var rightEye = new Vector2(center.X + eyeSpread, eyeY);
            dl.AddCircleFilled(leftEye,  eyeR, faceColor, 12);
            dl.AddCircleFilled(rightEye, eyeR, faceColor, 12);

            var mL = new Vector2(center.X - mouthHalf, mouthY);
            var mR = new Vector2(center.X + mouthHalf, mouthY);
            var c1 = new Vector2(center.X - mouthHalf * 0.6f, mouthY + mouthDepth);
            var c2 = new Vector2(center.X + mouthHalf * 0.6f, mouthY + mouthDepth);
            dl.AddBezierCubic(mL, c1, c2, mR, faceColor, Math.Max(1.5f, r * 0.08f));

            // ===== STOP 文本（白字 + 黑描边，向下移）=====
            const string text = "STOP";
            float targetFont = (float)Math.Clamp(r * 0.75f, 10.0, 128.0);
            var   font       = ImGui.GetFont();

            Vector2 baseSize = ImGui.CalcTextSize(text);
            float   scale    = targetFont / ImGui.GetFontSize();
            Vector2 textSize = baseSize * scale;

            // 精确居中，纵向下移避免遮挡笑脸
            Vector2 pos = new(
                center.X - textSize.X / 2f,
                center.Y - textSize.Y / 2f + r * 0.15f
            );

            float outline = (float)Math.Clamp(targetFont * 0.10f, 1.0, 4.0);
            for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                dl.AddText(font, targetFont, pos + new Vector2(dx * outline, dy * outline),
                           outerBorder, text);
            }
            dl.AddText(font, targetFont, pos, textColor, text);
        }
    }
}
