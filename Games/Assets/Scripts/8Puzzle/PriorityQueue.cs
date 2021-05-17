using System.Collections;
using System.Collections.Generic;

namespace Puzzle
{
    // A rudimentary PriorityQueue implementation.
    // it does not cater for performance or efficiency.
    public class PriorityQueue
    {
        // The items and priorities.
        List<Node> _nodes = new List<Node>();

        // Return the number of items in the queue.
        public int Count
        {
            get
            {
                return _nodes.Count;
            }
        }

        // Add an item to the queue.
        public void Add(Node n)
        {
            _nodes.Add(n);
        }

        // Get the item with the highest priority (in our case the lowest cost)
        public Node GetAndRemoveTop()
        {
            // Find the hightest priority.
            int best_index = 0;
            int best_priority = _nodes[0].Cost;
            for (int i = 1; i < _nodes.Count; i++)
            {
                if (best_priority > _nodes[i].Cost)
                {
                    best_priority = _nodes[i].Cost;
                    best_index = i;
                }
            }

            Node n = _nodes[best_index];
            _nodes.RemoveAt(best_index);

            return n;
        }
    }
}