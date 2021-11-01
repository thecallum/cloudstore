import styled from "styled-components";

import TaskBar from "./taskbar";
import StorageUsage from "../storageUsage";

const StyledMenu = styled.div`
  // border: 1px solid black;
  padding: 15px;
  background: hsl(180, 20%, 50%);

  display: flex;
  flex-direction: column;

  .taskbar {
    margin-bottom: 15px;

    button,
    li {
      width: 100%;
    }

    button {
      background: hsl(180, 20%, 30%);
      color: #fff;
      border-radius: 3px;
    }
  }

  @media (min-width: 500px) {
    flex-direction: row-reverse;
    justify-content: space-between;

    .taskbar {
      margin: 0;

      button,
      li {
        width: auto;
      }
    }

    .storageUsage {
      margin-right: 15px;
    }
  }
`;

const Menu = ({ directoryId, directory, storageUsage }) => {
  return (
    <StyledMenu>
      <TaskBar directoryId={directoryId} directory={directory} />

      <StorageUsage storageUsage={storageUsage} />
    </StyledMenu>
  );
};

export default Menu;
