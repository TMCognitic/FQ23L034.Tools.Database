namespace Tools.Database
{
    public class Command
    {
        /// <summary>
        /// Représente le texte de la requête
        /// </summary>
        internal string Query { get; init; }
        /// <summary>
        /// Définit s'il s'agit d'une procédure stockée ou non
        /// </summary>
        internal bool IsStoredProcedure { get; init; }
        /// <summary>
        /// Paramètre de la requête paramètrée
        /// </summary>
        internal IDictionary<string, object> Parameters { get; init; }

        public Command(string query, bool isStoredProcedure)
        {
            Query = query;
            IsStoredProcedure = isStoredProcedure;
            Parameters = new Dictionary<string, object>();
        }

        public void AddParameter(string parameterName, object? value)
        {
            ArgumentNullException.ThrowIfNull(parameterName, nameof(parameterName));

            Parameters.Add(parameterName, value ?? DBNull.Value);
        }
    }
}