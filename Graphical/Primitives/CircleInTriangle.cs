namespace Graphical.Primitives
{
    public record CircleInTriangle(
        float Size,
        string? Key = null,
        Transform? Transform = null,
        Paint? Paint = null
    ) : Composite(Key, Transform, Paint)
    {
        public override IEnumerable<Primitive> GetPrimitives()
        {
            return
            [
                new Triangle(
                    new(-Size, -Size),
                    new(0, Size),
                    new(Size, -Size),
                    Transform: Transform,
                    Paint: Paint
                ),
                new Circle(Size, Transform: Transform, Paint: Paint),
            ];
        }
    }
}
