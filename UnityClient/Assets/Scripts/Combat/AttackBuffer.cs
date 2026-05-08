using System.Collections.Generic;
using UnityEngine;
using StreetFighter.Data;

namespace StreetFighter.Combat
{
    /// <summary>
    /// Buffers attack inputs during recovery/cancel windows.
    /// </summary>
    public class AttackBuffer
    {
        private readonly Queue<string> buffer = new();
        private readonly float bufferTime = 0.2f; // Max buffer window

        public void BufferInput(string moveId)
        {
            buffer.Enqueue(moveId);
        }

        public string GetBufferedInput()
        {
            if (buffer.Count == 0) return null;
            return buffer.Dequeue();
        }

        public bool HasBufferedInput() => buffer.Count > 0;

        // Integrate with ComboChain cancel windows
    }
}
