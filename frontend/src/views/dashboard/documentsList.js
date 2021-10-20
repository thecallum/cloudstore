const DocumentsList = ({ documents, urlComponents }) => {
  return (
    <>
      <h2>Documents [{documents.length}]</h2>

      <ul>
        {documents.map((x, index) => (
          <li key={index}>{x.name}</li>
        ))}
      </ul>
    </>
  );
};

export default DocumentsList;
