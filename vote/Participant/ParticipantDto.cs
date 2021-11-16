namespace vote.Participant
{
    public record ParticipantDto
    {
        public ParticipantDto(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}