import HeaderLinks from "./headerLinks";
import styled from "styled-components";

const HeaderStyled = styled.header`
  background: hsl(180, 20%, 50%);
`;

const HeaderContainerStyled = styled.div`
  // padding: 30px;

  padding: 15px 0;
  display: flex;
  justify-content: space-between;
  flex-direction: column;

  width: calc(100% - 30px);
  max-width: 800px;
  margin: 0 auto;

  @media(min-width: 400px) {
    flex-direction: row;
    align-items: center;

  }

`;

const header = () => {


  return (
    <HeaderStyled>
      <HeaderContainerStyled>
        <div style={{
          fontSize: "26px"
        }}>CloudStore</div>

        <HeaderLinks />
      </HeaderContainerStyled>
    </HeaderStyled>
  );
};

export default header;
