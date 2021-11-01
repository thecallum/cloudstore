import CreateDirectoryModal from "./modals/createDirectoryModal";
import UploadDocumentModal from "./modals/uploadDocumentModal";
import DeleteDirectoryModal from "./modals/deleteDirectoryModal";
import RenameDirectoryModal from "./modals/renameDirectoryModal";

export const modalActions = {
  CREATE_DIRECTORY_MODAL: "CREATE_DIRECTORY_MODAL",
  UPLOAD_DOCUMENT_MODAL: "UPLOAD_DOCUMENT_MODAL",
  DELETE_DIRECTORY_MODAL: "DELETE_DIRECTORY_MODAL",
  RENAME_DIRECTORY_MODAL: "RENAME_DIRECTORY_MODAL",
};

export const createDirectoryAction = {
  text: "Create Directory",
  name: modalActions.CREATE_DIRECTORY_MODAL,
  component: CreateDirectoryModal,
};

export const uploadDocumentAction = {
  text: "Upload Document",
  name: modalActions.UPLOAD_DOCUMENT_MODAL,
  component: UploadDocumentModal,
};

export const deleteDirectoryAction = {
  text: "Delete Directory",
  name: modalActions.DELETE_DIRECTORY_MODAL,
  component: DeleteDirectoryModal,
};

export const renameDirectoryAction = {
  text: "Rename Directory",
  name: modalActions.RENAME_DIRECTORY_MODAL,
  component: RenameDirectoryModal,
};
