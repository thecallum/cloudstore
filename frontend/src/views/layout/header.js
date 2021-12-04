import HeaderLinks from "./headerLinks";

const header = () => {


  return (
    <header
      style={{
        background: "hsl(180, 20%, 50%)",
        padding: "30px",
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center",
      }}
    >
      <div>CloudStore</div>

      <HeaderLinks />
      
    </header>
  );
};

export default header;
