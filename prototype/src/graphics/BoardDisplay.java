package graphics;

import javax.swing.*;
import java.awt.*;
import java.awt.geom.Path2D;
import java.awt.geom.Point2D;
import java.util.Arrays;

public class BoardDisplay extends JPanel {
    private final int[] pointA;
    private final int[] pointB;
    private final int[] pointC;

    public BoardDisplay(int[] newPointA, int[] newPointB, int[] newPointC)
    {
        super();
        pointA = newPointA;
        pointB = newPointB;
        pointC = newPointC;
    }

    public void paintComponent(Graphics g)
    {
        Triangle triangle = new Triangle(new Point2D.Double(pointA[0], pointA[1]),
                new Point2D.Double(pointB[0], pointB[1]), new Point2D.Double(pointC[0], pointC[1]));
        Graphics2D g2d = (Graphics2D) g.create();
        g2d.setPaint(Color.ORANGE);
        g2d.draw(triangle);
    }

    @Override
    public String toString() {
        return "BoardDisplay{" +
                "pointA=" + Arrays.toString(pointA) +
                ", pointB=" + Arrays.toString(pointB) +
                ", pointC=" + Arrays.toString(pointC) +
                '}';
    }
}