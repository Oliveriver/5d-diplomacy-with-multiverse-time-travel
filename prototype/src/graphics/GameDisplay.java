package graphics;

import processing.*;

import javax.swing.*;
import java.awt.*;
import java.awt.geom.Path2D;
import java.awt.geom.Point2D;
import java.awt.geom.QuadCurve2D;
import java.util.ArrayList;
import java.util.Arrays;

public class GameDisplay extends JPanel {
    private ArrayList<Board> boards;
    private ArrayList<Army> armies;
    private ArrayList<Order> orders;

    public GameDisplay(ArrayList<Board> boardList, ArrayList<Army> armyList, ArrayList<Order> displayedOrders)
    {
        boards = boardList;
        armies = armyList;
        orders = displayedOrders;
    }

    @Override
    protected void paintComponent(Graphics g)
    {
        super.paintComponent(g);
        int side = 150;
        double spacing = 1.3;
        Graphics2D g2d = (Graphics2D) g.create();

        for (Board board : boards)
        {

            g2d.setColor(board.isActive() ? new Color(0,0,255,63) : new Color(0, 0, 255, 15));
            g2d.fill(new Province(
                    new Point2D.Double(
                            side / 2.0 + board.getPosition()[0] * side * spacing,
                            540 + (side / 6.0) * Math.sqrt(3) - board.getPosition()[1] * side * spacing),
                    new Point2D.Double(
                            side + board.getPosition()[0] * side * spacing,
                            540 + (side / 6.0) * Math.sqrt(3) - board.getPosition()[1] * side * spacing),
                    new Point2D.Double(
                            side + board.getPosition()[0] * side * spacing,
                            540 - board.getPosition()[1] * side * spacing),
                    new Point2D.Double(
                            3 * side / 4.0 + board.getPosition()[0] * side * spacing,
                            540 - (side / 12.0) * Math.sqrt(3) - board.getPosition()[1] * side * spacing)
            ));

            g2d.setColor(board.isActive() ? new Color(255,165,0,63) : new Color(255, 165, 0, 15));
            g2d.fill(new Province(
                    new Point2D.Double(
                            side + board.getPosition()[0] * side * spacing,
                            540 - board.getPosition()[1] * side * spacing),
                    new Point2D.Double(
                            side + board.getPosition()[0] * side * spacing,
                            540 + (side / 6.0) * Math.sqrt(3) - board.getPosition()[1] * side * spacing),
                    new Point2D.Double(
                            side * 3 / 2.0 + board.getPosition()[0] * side * spacing,
                            540 + (side / 6.0) * Math.sqrt(3) - board.getPosition()[1] * side * spacing),
                    new Point2D.Double(
                            5 * side / 4.0 + board.getPosition()[0] * side * spacing,
                            540 - (side / 12.0) * Math.sqrt(3) - board.getPosition()[1] * side * spacing)
            ));

            g2d.setPaint(board.isActive() ? Color.BLACK : new Color(0, 0, 0, 63));
            g2d.setStroke(new BasicStroke(2, BasicStroke.CAP_BUTT, BasicStroke.JOIN_ROUND, 0, null, 0));
            Triangle triangle = new Triangle(
                    new Point2D.Double(
                            side / 2.0 + board.getPosition()[0] * side * spacing,
                            540 + (side / 6.0) * Math.sqrt(3) - board.getPosition()[1] * side * spacing),
                    new Point2D.Double(
                            side * 3 / 2.0 + board.getPosition()[0] * side * spacing,
                            540 + (side / 6.0) * Math.sqrt(3) - board.getPosition()[1] * side * spacing),
                    new Point2D.Double(
                            side + board.getPosition()[0] * side * spacing,
                            540 - (side / 3.0) * Math.sqrt(3) - board.getPosition()[1] * side * spacing));
            g2d.draw(triangle);
            g2d.drawLine(
                    (int) (side + board.getPosition()[0] * side * spacing),
                    (int) (540 + (side / 6.0) * Math.sqrt(3) - board.getPosition()[1] * side * spacing),
                    (int) (side + board.getPosition()[0] * side * spacing),
                    (int) (540 - board.getPosition()[1] * side * spacing));
            g2d.drawLine(
                    (int) (5 * side / 4 + board.getPosition()[0] * side * spacing),
                    (int) (540 - (side / 12.0) * Math.sqrt(3) - board.getPosition()[1] * side * spacing),
                    (int) (side + board.getPosition()[0] * side * spacing),
                    (int) (540 - board.getPosition()[1] * side * spacing));
            g2d.drawLine(
                    (int) (3 * side / 4 + board.getPosition()[0] * side * spacing),
                    (int) (540 - (side / 12.0) * Math.sqrt(3) - board.getPosition()[1] * side * spacing),
                    (int) (side + board.getPosition()[0] * side * spacing),
                    (int) (540 - board.getPosition()[1] * side * spacing));
        }

        int armyScaling = 20;
        double offsetX, offsetY;
        for (Army army : armies) {
            switch (army.getOwner()) {
                case BLUE:
                    g2d.setColor(new Color(0, 0, 255));
                    break;
                case ORANGE:
                    g2d.setColor(new Color(255, 165, 0));
                    break;
                default:
                    g2d.setColor(Color.GRAY);
            }
            switch (army.getLocation()[2])
            {
                case 0:
                    offsetX = side / -4.0;
                    offsetY = side * Math.sqrt(3) / 12.0;
                    break;
                case 1:
                    offsetX = 0;
                    offsetY = side * Math.sqrt(3) / -6.0;
                    break;
                default:
                    offsetX = side / 4.0;
                    offsetY = side * Math.sqrt(3) / 12.0;
            }
            g2d.fillOval(
                    (int) (side + army.getLocation()[0] * side * spacing + offsetX - armyScaling / 2.0),
                    (int) (540 - army.getLocation()[1] * side * spacing + offsetY - armyScaling / 2.0),
                    armyScaling, armyScaling);
        }

        double offsetXLocation, offsetYLocation, offsetXDestination, offsetYDestination, offsetXSupport, offsetYSupport;
        for (Order order : orders)
        {
            g2d.setColor(order.getPlayer() == Player.BLUE ? new Color(0, 0, 255) : new Color(255, 165, 0));
            if (order instanceof Move)
            {
                g2d.setStroke(new BasicStroke(2, BasicStroke.CAP_BUTT, BasicStroke.JOIN_ROUND, 0, null, 0));
                Move move = (Move) order;
                switch (move.getLocation()[2])
                {
                    case 0:
                        offsetXLocation = side / -4.0;
                        offsetYLocation = side * Math.sqrt(3) / 12.0;
                        break;
                    case 1:
                        offsetXLocation = 0;
                        offsetYLocation = side * Math.sqrt(3) / -6.0;
                        break;
                    default:
                        offsetXLocation = side / 4.0;
                        offsetYLocation = side * Math.sqrt(3) / 12.0;
                }
                switch (move.getDestination()[2])
                {
                    case 0:
                        offsetXDestination = side / -4.0;
                        offsetYDestination = side * Math.sqrt(3) / 12.0;
                        break;
                    case 1:
                        offsetXDestination = 0;
                        offsetYDestination = side * Math.sqrt(3) / -6.0;
                        break;
                    default:
                        offsetXDestination = side / 4.0;
                        offsetYDestination = side * Math.sqrt(3) / 12.0;
                }
                drawArrowLine(g2d,
                        (int) (side + move.getLocation()[0] * side * spacing + offsetXLocation),
                        (int) (540 - move.getLocation()[1] * side * spacing + offsetYLocation),
                        (int) (side + move.getDestination()[0] * side * spacing + offsetXDestination),
                        (int) (540 - move.getDestination()[1] * side * spacing + offsetYDestination),
                        20, 10
                );
            }
            else if (order instanceof Support)
            {
                g2d.setStroke(new BasicStroke(2, BasicStroke.CAP_BUTT, BasicStroke.JOIN_ROUND, 0, new float[] {5}, 0));
                Support support = (Support) order;
                switch (support.getLocation()[2])
                {
                    case 0:
                        offsetXLocation = side / -4.0;
                        offsetYLocation = side * Math.sqrt(3) / 12.0;
                        break;
                    case 1:
                        offsetXLocation = 0;
                        offsetYLocation = side * Math.sqrt(3) / -6.0;
                        break;
                    default:
                        offsetXLocation = side / 4.0;
                        offsetYLocation = side * Math.sqrt(3) / 12.0;
                }
                switch (support.getSupportDestination()[2])
                {
                    case 0:
                        offsetXDestination = side / -4.0;
                        offsetYDestination = side * Math.sqrt(3) / 12.0;
                        break;
                    case 1:
                        offsetXDestination = 0;
                        offsetYDestination = side * Math.sqrt(3) / -6.0;
                        break;
                    default:
                        offsetXDestination = side / 4.0;
                        offsetYDestination = side * Math.sqrt(3) / 12.0;
                }
                switch (support.getSupportLocation()[2])
                {
                    case 0:
                        offsetXSupport = side / -4.0;
                        offsetYSupport = side * Math.sqrt(3) / 12.0;
                        break;
                    case 1:
                        offsetXSupport = 0;
                        offsetYSupport = side * Math.sqrt(3) / -6.0;
                        break;
                    default:
                        offsetXSupport = side / 4.0;
                        offsetYSupport = side * Math.sqrt(3) / 12.0;
                }
                if (Arrays.equals(support.getSupportLocation(), support.getSupportDestination()))
                {
                    g2d.drawLine(
                            (int) (side + support.getLocation()[0] * side * spacing + offsetXLocation),
                            (int) (540 - support.getLocation()[1] * side * spacing + offsetYLocation),
                            (int) (side + support.getSupportDestination()[0] * side * spacing + offsetXDestination),
                            (int) (540 - support.getSupportDestination()[1] * side * spacing + offsetYDestination)
                    );
                    g2d.drawOval(
                            (int) (side + support.getSupportDestination()[0] * side * spacing + offsetXDestination - armyScaling / 2.0 - 3),
                            (int) (540 - support.getSupportDestination()[1] * side * spacing + offsetYDestination - armyScaling / 2.0 - 3),
                            armyScaling + 6, armyScaling + 6
                    );
                }
                else
                {
                    QuadCurve2D curve = new QuadCurve2D.Float();
                    curve.setCurve(
                            (int) (side + support.getLocation()[0] * side * spacing + offsetXLocation),
                            (int) (540 - support.getLocation()[1] * side * spacing + offsetYLocation),
                            (int) (side + support.getSupportLocation()[0] * side * spacing + offsetXSupport),
                            (int) (540 - support.getSupportLocation()[1] * side * spacing + offsetYSupport),
                            (int) (side + support.getSupportDestination()[0] * side * spacing + offsetXDestination),
                            (int) (540 - support.getSupportDestination()[1] * side * spacing + offsetYDestination)
                    );
                    g2d.draw(curve);
                }
            }
        }
    }

    // Shamelessly copied straight from: https://stackoverflow.com/questions/2027613/how-to-draw-a-directed-arrow-line-in-java
    private void drawArrowLine(Graphics g, int x1, int y1, int x2, int y2, int d, int h) {
        int dx = x2 - x1, dy = y2 - y1;
        double D = Math.sqrt(dx*dx + dy*dy);
        double xm = D - d, xn = xm, ym = h, yn = -h, x;
        double sin = dy / D, cos = dx / D;

        x = xm*cos - ym*sin + x1;
        ym = xm*sin + ym*cos + y1;
        xm = x;

        x = xn*cos - yn*sin + x1;
        yn = xn*sin + yn*cos + y1;
        xn = x;

        int[] xpoints = {x2, (int) xm, (int) xn};
        int[] ypoints = {y2, (int) ym, (int) yn};

        g.drawLine(x1, y1, x2, y2);
        g.fillPolygon(xpoints, ypoints, 3);
    }
}

class Triangle extends Path2D.Double {
    public Triangle(Point2D... points)
    {
        moveTo(points[0].getX(), points[0].getY());
        lineTo(points[1].getX(), points[1].getY());
        lineTo(points[2].getX(), points[2].getY());
        closePath();
    }
}

class Province extends Path2D.Double {
    public Province(Point2D... points)
    {
        moveTo(points[0].getX(), points[0].getY());
        lineTo(points[1].getX(), points[1].getY());
        lineTo(points[2].getX(), points[2].getY());
        lineTo(points[3].getX(), points[3].getY());
        closePath();
    }
}