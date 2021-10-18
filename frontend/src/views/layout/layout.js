import Header from "./header";

const layout = ({ children }) => {
  return (
    <>
      <Header />

      <main>
        <div
          style={{
            width: "calc(100% - 30px)",
            maxWidth: "800px",
            margin: "50px auto",
          }}
        >
          {children}
        </div>
      </main>
    </>
  );
};

export default layout;
