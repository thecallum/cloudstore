import fileSize from "filesize.js";

const storageUsage = ({ storageUsage }) => {
  if (storageUsage === null) return null;

  return (
    <div className="storageUsage">
      <h2>Storage Usage</h2>

      <div>
        {fileSize(storageUsage.storageUsage)} used of{" "}
        {fileSize(storageUsage.capacity)}
      </div>

      <div>
        {Math.floor((storageUsage.storageUsage / storageUsage.capacity) * 100)}%
        Usage
      </div>

      {/* <pre>{JSON.stringify(storageUsage, null, 2)}</pre> */}
    </div>
  );
};

export default storageUsage;
