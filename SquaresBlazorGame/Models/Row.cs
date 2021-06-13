using System.Collections.Generic;

namespace SquaresBlazorGame.Models
{
    public class Row
    {
        public int RowNumber { get;set; }
        public List<Box> Boxes {get;set;}

        public Row(int rowNumber, List<Box> boxes)
        {
            RowNumber = rowNumber;
            Boxes = boxes;
        }
    }
}
