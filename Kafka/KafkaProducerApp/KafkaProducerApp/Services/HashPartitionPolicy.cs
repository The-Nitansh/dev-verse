using KafkaProducerApp.Interfaces;

namespace KafkaProducerApp.Services;

public class HashPartitionPolicy : IPartitionPolicy
{
    private readonly Dictionary<string, List<int>> PartitionMapping;

    public HashPartitionPolicy(PartitionConfigProvider configProvider)
    {
        PartitionMapping = configProvider
            .GetPartitionMappings()
            .ToDictionary(x => x.UtilityName, x => x.Partitions);
    }

    public int ResolvePartition(string utilityName, string messageKey)
    {
        if (!PartitionMapping.TryGetValue(utilityName, out var partitions) || partitions.Count == 0)
        {
            throw new ArgumentException($"No partitions found for utility '{utilityName}'");
        }

        int hash = Math.Abs(GetDeterministicHashCode(messageKey));
        int index = hash % partitions.Count;
        return partitions[index];
    }

    private int GetDeterministicHashCode(string str)
    {
        unchecked
        {
            int hash = 17;
            foreach (char c in str)
            {
                hash = hash * 31 + c;
            }
            return hash;
        }
    }
}
